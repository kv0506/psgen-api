using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using PsGenApi.DataModel;
using PsGenApi.Middleware;
using PsGenApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.AllowTrailingCommas = true;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.PropertyNameCaseInsensitive = true;
});

// Configure EF Core with PostgreSQL
builder.Services.AddDbContext<PsGenDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("PsGenApi");
            sqlOptions.EnableRetryOnFailure(5);
        }));

// Register Services
builder.Services.AddTransient<IDatabaseService, DatabaseService>();
builder.Services.AddTransient<IRepositoryService, RepositoryService>();

// Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add health checks
builder.Services.AddHealthChecks();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PsGen API", Version = "v1" });
    
    // Configure Swagger to use JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Apply database migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbService = services.GetRequiredService<IDatabaseService>();
        dbService.EnsureDatabaseCreatedAsync().GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use the custom AuthToken middleware
app.UseMiddleware<AuthTokenMiddleware>();

// Map health check endpoint
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();