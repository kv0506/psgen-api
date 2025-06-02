using PsGenApi.Document;
using PsGenApi.Entity;
using PsGenApi.Services;

namespace PsGenApi;

public class AccountMigration
{
	private readonly ILogger<AccountMigration> _logger;
	private readonly IRepositoryService _repositoryService;
	private readonly TableService _tableService;

	public AccountMigration(
		TableService tableService,
		IRepositoryService repositoryService,
		ILogger<AccountMigration> logger)
	{
		_tableService = tableService;
		_repositoryService = repositoryService;
		_logger = logger;
	}

	public async Task<MigrationResult> MigrateAccountsAsync()
	{
		var result = new MigrationResult();

		var tableClient = _tableService.CreateTableClient();

		var tableAccounts = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == "Accounts");

		await foreach (var tableAccount in tableAccounts)
			try
			{
				var account = SerializationService.Deserialize<Account>(tableAccount.Document);

				if (account != null)
				{
					// Get accounts for this user from PostgreSQL
					var dbAccount = await _repositoryService.GetAccountByIdAsync(account.Id);
					if (dbAccount == null)
						try
						{
							// Save account to PostgreSQL
							await _repositoryService.CreateAccountAsync(account);
							result.SuccessCount++;
							_logger.LogInformation($"Migrated account {account.Id} for user {account.UserId}");
						}
						catch (Exception ex)
						{
							result.FailureCount++;
							result.Errors.Add($"Failed to migrate account {account.Id}: {ex.Message}");
							_logger.LogError(ex, $"Failed to migrate account {account.Id} for user {account.UserId}");
						}
				}
			}
			catch (Exception ex)
			{
				result.Errors.Add("Failed to process");
				_logger.LogError(ex, "Exception while processing");
			}

		return result;
	}
}

public class MigrationResult
{
	public int SuccessCount { get; set; }
	public int FailureCount { get; set; }
	public int SkippedCount { get; set; }
	public List<string> Errors { get; set; } = new();
}