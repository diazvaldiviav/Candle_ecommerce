using Candle_API.CoreSettings;
using Candle_API.Data.Entities;
using Candle_API.Middleware;
using Candle_API.Services.Implementations;
using Candle_API.Services.Interfaces;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuracion de la bd en memoria para desarrollo
builder.Services.AddDbContext<CandleDbContext>(options =>
    options.UseInMemoryDatabase("CandleShopDb"));

//mapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//memory cache
builder.Services.AddMemoryCache();

// Add the inyection of the CandleDbContext
builder.Services.AddScoped<ICategories, CategoryService>();
builder.Services.AddScoped<IProduct, ProductServices>();



//cookies
// Configurar cookie policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

// Configurar CORS desde appsettings
builder.Services.Configure<CorsSettings>(
    builder.Configuration.GetSection("Cors"));

var app = builder.Build();

// Agregar middlewares de seguridad
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<CustomCorsMiddleware>();
app.UseMiddleware<AntiXssMiddleware>();
app.UseMiddleware<AntiSqlInjectionMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CandleDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Primero las redirecciones HTTPS
app.UseHttpsRedirection();

// Después la autenticación y autorización
app.UseAuthentication(); // Agregar esta línea
app.UseAuthorization();


// Finalmente el routing y endpoints
app.MapControllers();

app.Run();