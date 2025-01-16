using Candle_API.CoreSettings;
using Microsoft.Extensions.Options;

namespace Candle_API.Middleware
{
    // Middleware/CustomCorsMiddleware.cs
    public class CustomCorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CorsSettings _corsSettings;
        private readonly ILogger<CustomCorsMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public CustomCorsMiddleware(
            RequestDelegate next,
            IOptions<CorsSettings> corsSettings,
            ILogger<CustomCorsMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _corsSettings = corsSettings.Value;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
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

                // Configurar headers CORS
                context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                context.Response.Headers.Add("Access-Control-Allow-Methods",
                    string.Join(",", _corsSettings.AllowedMethods));
                context.Response.Headers.Add("Access-Control-Allow-Headers",
                    string.Join(",", _corsSettings.AllowedHeaders));
                context.Response.Headers.Add("Access-Control-Expose-Headers",
                    string.Join(",", _corsSettings.ExposedHeaders));
                context.Response.Headers.Add("Access-Control-Max-Age",
                    _corsSettings.MaxAge.ToString());

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
