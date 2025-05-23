namespace PsGenApi.Services;

public interface IDatabaseService
{
    /// <summary>
    /// Ensures the database is created and migrated to the latest version
    /// </summary>
    Task EnsureDatabaseCreatedAsync();
}
