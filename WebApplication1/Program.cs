using Candle_API.CoreSettings;
using Candle_API.Middleware;
using Candle_API.Services.Implementations;
using Candle_API.Services.Interfaces;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuracion de la bd en memoria para desarrollo
//builder.Services.AddDbContext<CandleDbContext>(options =>
//  options.UseInMemoryDatabase("CandleShopDb"));


//configuracion sql server  
builder.Services.AddDbContext<CandleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//mapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//memory cache
builder.Services.AddMemoryCache();

// Add the inyection of the CandleDbContext
builder.Services.AddScoped<ICategories, CategoryService>();
builder.Services.AddScoped<IProduct, ProductServices>();
builder.Services.AddScoped<IColors, ColorServices>();
builder.Services.AddScoped<ISizes, SizeServices>();
builder.Services.AddScoped<ISubCategories, SubCategoriesServices>();



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


// Program.cs
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Candle API",
        Version = "v1",
        Description = "API para el e-commerce de velas aromáticas",
        Contact = new OpenApiContact
        {
            Name = "Tu Nombre",
            Email = "tu@email.com",
            Url = new Uri("https://tuwebsite.com")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Configurar la ruta al archivo XML de documentación
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Configurar la interfaz de Swagger
    options.EnableAnnotations();
    options.UseInlineDefinitionsForEnums();

    // Agrupar endpoints por controlador
    options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    options.DocInclusionPredicate((docName, description) => true);
});

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