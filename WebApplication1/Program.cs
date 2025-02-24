using Candle_API.CoreSettings;
using Candle_API.Middleware;
using Candle_API.Services.Implementations;
using Candle_API.Services.Interfaces;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuracion de la bd en memoria para desarrollo
//builder.Services.AddDbContext<CandleDbContext>(options =>
  //options.UseInMemoryDatabase("CandleShopDb"));


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
builder.Services.AddScoped<IAromas, AromaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();

//authenticacion
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Configuraci�n de las pol�ticas de autorizaci�n
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("RequireUserRole", policy =>
        policy.RequireRole("User", "Admin")); // Admin tambi�n puede acceder a rutas de User
});



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

//JSON Serializer
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });


// Program.cs
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Candle API",
        Version = "v1",
        Description = "API para el e-commerce de velas arom�ticas",
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

    // Configuraci�n del Bearer token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Configurar la ruta al archivo XML de documentaci�n
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

// Despu�s la autenticaci�n y autorizaci�n
app.UseAuthentication(); // Agregar esta l�nea
app.UseAuthorization();


// Finalmente el routing y endpoints
app.MapControllers();

app.Run();