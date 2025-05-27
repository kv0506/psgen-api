using PsGenApi.Document;
using PsGenApi.Services;
using PsGenApi.Entity;

namespace PsGenApi;

public class AccountMigration
{
	private readonly TableService _tableService;
	private readonly IRepositoryService _repositoryService;
	private readonly ILogger<AccountMigration> _logger;

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

		var tableUsers = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == "Users");

		await foreach (var tableUser in tableUsers)
		{
			try
			{
				var user = SerializationService.Deserialize<User>(tableUser.Document);

				if (user != null)
				{
					// Get users from PostgreSQL
					var dbUser = await _repositoryService.GetUserByIdAsync(user.Id);
					if (dbUser == null)
					{
						try
						{
							// Save user to PostgreSQL
							await _repositoryService.CreateUserAsync(user);
							_logger.LogInformation($"Migrated user {user.Id} for user {user.Username}");
						}
						catch (Exception ex)
						{
							result.FailureCount++;
							result.Errors.Add($"Failed to migrate account {user.Id}: {ex.Message}");
							_logger.LogError(ex, $"Failed to migrate user {user.Id} for user {user.Username}");
						}
					}
				}
			}
			catch (Exception ex)
			{
				result.Errors.Add("Failed to process users");
				_logger.LogError(ex, "Exception while processing user migration");
			}
		}

		var tableAccounts = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == "Accounts");

		await foreach (var tableAccount in tableAccounts)
		{
			try
			{
				var account = SerializationService.Deserialize<Account>(tableAccount.Document);

				if (account != null)
				{
					// Get accounts for this user from PostgreSQL
					var dbAccount = await _repositoryService.GetAccountByIdAsync(account.Id);
					if (dbAccount == null)
					{
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
			}
			catch (Exception ex)
			{
				result.Errors.Add("Failed to process accounts");
				_logger.LogError(ex, "Exception while processing account migration");
			}
		}

		return result;
	}
}

public class MigrationResult
{
	public int SuccessCount { get; set; }
	public int FailureCount { get; set; }
	public int SkippedCount { get; set; }
	public List<string> Errors { get; set; } = new List<string>();
}
