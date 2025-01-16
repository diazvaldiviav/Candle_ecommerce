// Middleware/AntiXssMiddleware.cs
using System.Text.RegularExpressions;
using System.Text;

namespace Candle_API.Middleware;

public class AntiXssMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AntiXssMiddleware> _logger;
    private readonly int _maxLength;

    public AntiXssMiddleware(RequestDelegate next, ILogger<AntiXssMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _maxLength = 4096; // Tamaño máximo permitido para el body
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Validar solo requests POST, PUT, PATCH
        if (HttpMethods.IsPost(context.Request.Method) ||
            HttpMethods.IsPut(context.Request.Method) ||
            HttpMethods.IsPatch(context.Request.Method))
        {
            var originalBody = context.Request.Body;
            try
            {
                var content = await ReadRequestBody(context.Request);

                // Validar contenido contra XSS
                if (ContainsXss(content))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Contenido potencialmente peligroso detectado"
                    });
                    return;
                }

                // Reemplazar el body para que pueda ser leído nuevamente
                var modifiedStream = new MemoryStream();
                var streamWriter = new StreamWriter(modifiedStream);
                await streamWriter.WriteAsync(content);
                await streamWriter.FlushAsync();
                modifiedStream.Position = 0;
                context.Request.Body = modifiedStream;

                await _next(context);
            }
            finally
            {
                context.Request.Body = originalBody;
            }
        }
        else
        {
            await _next(context);
        }
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        var buffer = new byte[_maxLength];
        var bytesRead = await request.Body.ReadAsync(buffer, 0, _maxLength);
        var content = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        request.Body.Position = 0;

        return content;
    }

    private bool ContainsXss(string content)
    {
        // Lista de patrones XSS comunes
        var xssPatterns = new[]
        {
            @"<script[^>]*>",
            @"javascript:",
            @"onload=",
            @"onerror=",
            @"onclick=",
            @"alert\(",
            @"eval\(",
            // Agregar más patrones según sea necesario
        };

        return xssPatterns.Any(pattern =>
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
    }
}