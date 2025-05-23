using PsGenApi.DataModel;

namespace PsGenApi.Services;

public class DatabaseService : IDatabaseService
{
    private readonly PsGenDbContext _dbContext;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(PsGenDbContext dbContext, ILogger<DatabaseService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Ensures the database is created and migrated to the latest version
    /// </summary>
    public async Task EnsureDatabaseCreatedAsync()
    {
        try
        {
            _logger.LogInformation("Checking database connection and migrations");

            await _dbContext.Database.MigrateAsync();

            _logger.LogInformation("Database is up to date");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while ensuring the database is created");
            throw;
        }
    }
}
