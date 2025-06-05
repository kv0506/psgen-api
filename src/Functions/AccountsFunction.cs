using CSharpExtensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PsGenApi.Document;
using PsGenApi.ServiceModel;
using PsGenApi.Services;

namespace PsGenApi.Functions;

public class AccountsFunction(ILoggerFactory loggerFactory, TableService tableService) : FunctionBase
{
	private readonly ILogger _logger = loggerFactory.CreateLogger<AccountsFunction>();

	[Function("accounts")]
	public async Task<HttpResponseData> Run(
		[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete")] HttpRequestData req)
	{
		_logger.LogInformation("Executing accounts function");

		if (!req.Headers.Contains("AuthToken")) return await AuthError(req, "AuthToken header is missing");

		var token = req.Headers.GetValues("AuthToken").FirstOrDefault();
		if (token.IsNullOrWhiteSpace()) return await AuthError(req, "AuthToken is empty");

		var tokenDoc = await tableService.GetTokenDocumentAsync(token!);
		if (tokenDoc == null || tokenDoc.ExpiresAt < DateTimeOffset.UtcNow)
			return await AuthError(req, "Invalid AuthToken");

		var httpMethod = req.Method.ToLowerInvariant();

		if (httpMethod == "put") return await HandleCreateAccountAsync(req, tokenDoc);

		if (httpMethod == "post") return await HandleUpdateAccountAsync(req, tokenDoc);

		if (httpMethod == "delete") return await HandleDeleteAccountAsync(req, tokenDoc);

		if (httpMethod == "get") return await HandleGetAccountAsync(req, tokenDoc);

		return await HttpMethodNotSupportedError(req);
	}

	private async Task<HttpResponseData> HandleCreateAccountAsync(HttpRequestData req, Token tokenDoc)
	{
		var createAccountReq = await ReadRequestBody<CreateAccountDto>(req);
		if (createAccountReq != null)
		{
			var account = new Account
			{
				Id = Guid.NewGuid().ToString("N"),
				UserId = tokenDoc.UserId,
				Name = createAccountReq.Name,
				Category = createAccountReq.Category,
				Username = createAccountReq.Username,
				Pattern = createAccountReq.Pattern,
				Length = createAccountReq.Length,
				IncludeSpecialCharacter = createAccountReq.IncludeSpecialCharacter,
				UseCustomSpecialCharacter = createAccountReq.UseCustomSpecialCharacter,
				CustomSpecialCharacter = createAccountReq.CustomSpecialCharacter,
				Notes = createAccountReq.Notes,
				IsFavorite = createAccountReq.IsFavorite
			};

			await tableService.CreateOrUpdateAccountDocumentAsync(tokenDoc.UserId, account);

			var apiResponse = new RecordResponseDto<AccountDto>
			{
				IsSuccess = true,
				Result = MapToDto(account)
			};

			return await Success(req, apiResponse);
		}

		return await InvalidRequestPayloadError(req);
	}

	private async Task<HttpResponseData> HandleUpdateAccountAsync(HttpRequestData req, Token tokenDoc)
	{
		var updateAccountReq = await ReadRequestBody<UpdateAccountDto>(req);
		if (updateAccountReq != null)
		{
			var account = await tableService.GetAccountDocumentAsync(updateAccountReq.Id);
			if (account == null || account.UserId.IsNotEquals(tokenDoc.UserId))
				return await Error(req, "Account does not exist");

			account.Name = updateAccountReq.Name;
			account.Category = updateAccountReq.Category;
			account.Username = updateAccountReq.Username;
			account.Pattern = updateAccountReq.Pattern;
			account.Length = updateAccountReq.Length;
			account.IncludeSpecialCharacter = updateAccountReq.IncludeSpecialCharacter;
			account.UseCustomSpecialCharacter = updateAccountReq.UseCustomSpecialCharacter;
			account.CustomSpecialCharacter = updateAccountReq.CustomSpecialCharacter;
			account.Notes = updateAccountReq.Notes;
			account.IsFavorite = updateAccountReq.IsFavorite;

			await tableService.CreateOrUpdateAccountDocumentAsync(tokenDoc.UserId, account);

			var apiResponse = new RecordResponseDto<AccountDto>
			{
				IsSuccess = true,
				Result = MapToDto(account)
			};

			return await Success(req, apiResponse);
		}

		return await InvalidRequestPayloadError(req);
	}

	private async Task<HttpResponseData> HandleDeleteAccountAsync(HttpRequestData req, Token tokenDoc)
	{
		var deleteAccountReq = await ReadRequestBody<DeleteAccountDto>(req);
		if (deleteAccountReq != null)
		{
			var account = await tableService.GetAccountDocumentAsync(deleteAccountReq.Id);
			if (account == null || account.UserId.IsNotEquals(tokenDoc.UserId))
				return await Error(req, "Account does not exist");

			await tableService.DeleteAccountDocumentAsync(deleteAccountReq.Id);
			return await Success(req, new DeletedResponseDto { IsSuccess = true, Result = true });
		}

		return await InvalidRequestPayloadError(req);
	}

	private async Task<HttpResponseData> HandleGetAccountAsync(HttpRequestData req, Token tokenDoc)
	{
		var accountId = req.Query.Get("accountId");
		if (accountId.IsNotNullOrWhiteSpace())
		{
			var account = await tableService.GetAccountDocumentAsync(accountId!);
			if (account == null || account.UserId.IsNotEquals(tokenDoc.UserId))
				return await Error(req, "Account does not exist");

			var apiResponse = new RecordResponseDto<AccountDto>
			{
				IsSuccess = true,
				Result = MapToDto(account)
			};

			return await Success(req, apiResponse);
		}
		else
		{
			var accounts = await tableService.GetAccountDocumentsAsync(tokenDoc.UserId);
			accounts = accounts.OrderBy(x => x.Category).ThenBy(x => x.Name).ToList();
			var apiResponse = new RecordsResponseDto<AccountDto>
			{
				IsSuccess = true,
				Result = accounts.Select(MapToDto).ToList()
			};

			return await Success(req, apiResponse);
		}
	}

	private AccountDto MapToDto(Account account)
	{
		return new AccountDto
		{
			Id = account.Id,
			Name = account.Name,
			Category = account.Category,
			Username = account.Username,
			Pattern = account.Pattern,
			Length = account.Length,
			IncludeSpecialCharacter = account.IncludeSpecialCharacter,
			UseCustomSpecialCharacter = account.UseCustomSpecialCharacter,
			CustomSpecialCharacter = account.CustomSpecialCharacter,
			Notes = account.Notes,
			IsFavorite = account.IsFavorite
		};
	}
}