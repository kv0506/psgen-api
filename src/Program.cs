using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using PsGenApi.DataModel;
using PsGenApi.Services;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureAppConfiguration(builder => { builder.AddEnvironmentVariables(); })
	.ConfigureServices((context, services) =>
	{
		services.Configure<JsonSerializerOptions>(options =>
		{
			options.AllowTrailingCommas = true;
			options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			options.PropertyNameCaseInsensitive = true;
		});

		// Configure EF Core with PostgreSQL
		services.AddDbContext<PsGenDbContext>(options =>
			options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection"),
				sqlOptions =>
				{					sqlOptions.MigrationsAssembly("PsGenApi");
					sqlOptions.EnableRetryOnFailure(5);
				})); 
		
		// Register Services
		services.AddTransient<IDatabaseService, DatabaseService>();
		services.AddTransient<IRepositoryService, RepositoryService>();
	}).Build();

// Apply database migrations at startup
using (var scope = host.Services.CreateScope())
{	var services = scope.ServiceProvider;
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

host.Run();