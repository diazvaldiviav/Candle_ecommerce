using Candle_API.CoreSettings;
using Microsoft.Extensions.Configuration;

namespace Candle_API.Middleware
{
    // Middleware/SecurityHeadersMiddleware.cs
    // Middleware/SecurityHeadersMiddleware.cs
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public SecurityHeadersMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _next = next;
            _env = env;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Obtener los orígenes permitidos de la configuración
            var corsSettings = _configuration.GetSection("Cors").Get<CorsSettings>();
            var allowedOrigins = string.Join(" ", corsSettings?.AllowedOrigins ?? Array.Empty<string>());

            if (_env.IsDevelopment())
            {
                context.Response.Headers.Add(
                    "Content-Security-Policy",
                    $"default-src 'self' {allowedOrigins}; " +
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data: blob: {allowedOrigins}; " +
                    "font-src 'self' data:; " +
                    $"connect-src 'self' {allowedOrigins} ws: wss:; " +
                    "media-src 'self' blob: data:; " +
                    "object-src 'none';"
                );
            }
            else
            {
                context.Response.Headers.Add(
                    "Content-Security-Policy",
                    $"default-src 'self' {allowedOrigins}; " +
                    "script-src 'self'; " +
                    "style-src 'self'; " +
                    $"img-src 'self' data: {allowedOrigins}; " +
                    "font-src 'self'; " +
                    $"connect-src 'self' {allowedOrigins}; " +
                    "media-src 'self'; " +
                    "object-src 'none';"
                );
            }

            // Otras cabeceras de seguridad
            context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

            await _next(context);
        }
    }
}
