using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
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
		// Register Services
		services.AddTransient<TableService>();
	}).Build();

host.Run();