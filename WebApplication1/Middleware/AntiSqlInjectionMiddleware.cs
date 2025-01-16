// Middleware/AntiSqlInjectionMiddleware.cs
using System.Text.RegularExpressions;

namespace Candle_API.Middleware;

public class AntiSqlInjectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AntiSqlInjectionMiddleware> _logger;

    public AntiSqlInjectionMiddleware(RequestDelegate next, ILogger<AntiSqlInjectionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method) ||
            HttpMethods.IsPut(context.Request.Method) ||
            HttpMethods.IsPatch(context.Request.Method))
        {
            var originalBody = context.Request.Body;
            try
            {
                var content = await ReadRequestBody(context.Request);

                if (ContainsSqlInjection(content))
                {
                    _logger.LogWarning("Posible intento de SQL Injection detectado");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Contenido no permitido detectado"
                    });
                    return;
                }

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

    private bool ContainsSqlInjection(string content)
    {
        var sqlPatterns = new[]
        {
            @";\s*DROP\s+TABLE",
            @";\s*DELETE\s+FROM",
            @";\s*INSERT\s+INTO",
            @";\s*UPDATE\s+",
            @"'\s*OR\s+'1'\s*=\s*'1",
            @"--\s*$",
            @"\/\*.*\*\/",
            // Agregar más patrones según sea necesario
        };

        return sqlPatterns.Any(pattern =>
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        var reader = new StreamReader(request.Body);
        var content = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return content;
    }
}