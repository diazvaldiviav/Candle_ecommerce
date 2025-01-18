using Candle_API.CoreSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Candle_API.Middleware
{
    // Middleware/CustomCorsMiddleware.cs
    public class CustomCorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CorsSettings _corsSettings;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomCorsMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public CustomCorsMiddleware(
            RequestDelegate next,
            IOptions<CorsSettings> corsSettings,
            ILogger<CustomCorsMiddleware> logger,
            IWebHostEnvironment environment,
            IConfiguration configuration)
           
        {
            _next = next;
            _corsSettings = corsSettings.Value;
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var corsSettings = _configuration.GetSection("Cors").Get<CorsSettings>() ?? new CorsSettings
            {
                AllowedOrigins = new[] { "*" }, // Valor por defecto para desarrollo
                AllowedMethods = new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" },
                AllowedHeaders = new[] { "Content-Type", "Authorization", "Accept" }
            };
            var origin = context.Request.Headers.Origin.ToString();

            if (!string.IsNullOrEmpty(origin))
            {
                // Verificar si el origen está permitido
                if (!IsOriginAllowed(origin))
                {
                    _logger.LogWarning("Intento de acceso bloqueado desde origen no permitido: {Origin}", origin);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Origen no permitido"
                    });
                    return;
                }

                if (_environment.IsDevelopment())
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                    context.Response.Headers.Add(
                        "Access-Control-Allow-Methods",
                        string.Join(", ", corsSettings.AllowedMethods ?? Array.Empty<string>()));
                    context.Response.Headers.Add(
                        "Access-Control-Allow-Headers",
                        string.Join(", ", corsSettings.AllowedHeaders ?? Array.Empty<string>()));
                }
                else
                {
                    var allowedOrigins = corsSettings.AllowedOrigins ?? Array.Empty<string>();
                    if (allowedOrigins.Contains(origin))
                    {
                        context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                        context.Response.Headers.Add(
                            "Access-Control-Allow-Methods",
                            string.Join(", ", corsSettings.AllowedMethods ?? Array.Empty<string>()));
                        context.Response.Headers.Add(
                            "Access-Control-Allow-Headers",
                            string.Join(", ", corsSettings.AllowedHeaders ?? Array.Empty<string>()));
                    }
                }

                // Manejar preflight requests
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    return;
                }
            }

            await _next(context);
        }

        private bool IsOriginAllowed(string origin)
        {
            // En desarrollo, permitir todos los orígenes configurados
            if (_environment.IsDevelopment())
            {
                return _corsSettings.AllowedOrigins.Contains(origin);
            }

            // En producción, validación estricta
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
            {
                return false;
            }

            // Validar protocolo HTTPS en producción
            if (uri.Scheme != "https")
            {
                return false;
            }

            // Validar contra lista de dominios permitidos
            return _corsSettings.AllowedOrigins.Any(allowed =>
            {
                if (Uri.TryCreate(allowed, UriKind.Absolute, out var allowedUri))
                {
                    return uri.Host.Equals(allowedUri.Host,
                        StringComparison.OrdinalIgnoreCase);
                }
                return false;
            });
        }
    }
}
