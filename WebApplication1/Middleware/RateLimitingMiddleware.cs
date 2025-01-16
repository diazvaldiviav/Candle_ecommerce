// Middleware/RateLimitingMiddleware.cs
using Microsoft.Extensions.Caching.Memory;


namespace Candle_API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    // Configuración del rate limiting
    private readonly int _maxRequests = 100; // máximo de requests por ventana de tiempo
    private readonly int _timeWindowMinutes = 1; // ventana de tiempo en minutos

    public RateLimitingMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var cacheKey = $"rate_limit_{ipAddress}";

        // Obtener o crear el contador de requests
        var requestCount = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_timeWindowMinutes);
            return 0;
        });

        if (requestCount > _maxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for IP: {IpAddress}", ipAddress);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Demasiadas solicitudes. Por favor, intente más tarde."
            });
            return;
        }

        _cache.Set(cacheKey, requestCount + 1,
            TimeSpan.FromMinutes(_timeWindowMinutes));

        await _next(context);
    }
}