using Microsoft.EntityFrameworkCore.Design;

namespace PsGenApi.DataModel;

/// <summary>
///     This class is used by EF Core tools to create migrations.
///     It's not used during runtime.
/// </summary>
public class PsGenDbContextFactory : IDesignTimeDbContextFactory<PsGenDbContext>
{
	public PsGenDbContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("local.settings.json", true, true)
			.AddEnvironmentVariables()
			.Build();

		var optionsBuilder = new DbContextOptionsBuilder<PsGenDbContext>();
		optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
			sqlOptions =>
			{
				sqlOptions.MigrationsAssembly("PsGenApi");
				sqlOptions.EnableRetryOnFailure(5);
			});

		return new PsGenDbContext(optionsBuilder.Options);
	}
}