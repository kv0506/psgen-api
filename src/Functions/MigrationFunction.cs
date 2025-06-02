using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PsGenApi.ServiceModel;

namespace PsGenApi.Functions;

public class MigrationFunction : FunctionBase
{
	private readonly AccountMigration _accountMigration;
	private readonly ILogger _logger;

	public MigrationFunction(
		ILoggerFactory loggerFactory,
		AccountMigration accountMigration)
	{
		_logger = loggerFactory.CreateLogger<MigrationFunction>();
		_accountMigration = accountMigration;
	}

	[Function("migrate-data")]
	public async Task<HttpResponseData> Run(
		[HttpTrigger(AuthorizationLevel.Admin, "post")]
		HttpRequestData req)
	{
		_logger.LogInformation("Account migration function started");

		try
		{
			var result = await _accountMigration.MigrateAccountsAsync();

			var response = new
			{
				result.SuccessCount,
				result.FailureCount,
				result.SkippedCount,
				result.Errors
			};

			return await Success(req, new ApiResponseDto
			{
				IsSuccess = result.FailureCount == 0,
				Message =
					$"Migration completed: {result.SuccessCount} accounts migrated, {result.SkippedCount} skipped, {result.FailureCount} failed",
				Result = response
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in account migration");
			return await Error(req, $"Migration failed: {ex.Message}");
		}
	}
}