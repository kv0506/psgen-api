using CSharpExtensions;
using Microsoft.AspNetCore.Mvc;
using PsGenApi.Document;
using PsGenApi.Middleware;
using PsGenApi.ServiceModel;
using PsGenApi.Services;

namespace PsGenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : BaseController
{
	private readonly ILogger<AccountsController> _logger;
	private readonly IRepositoryService _repositoryService;

	public AccountsController(ILogger<AccountsController> logger, IRepositoryService repositoryService)
	{
		_logger = logger;
		_repositoryService = repositoryService;
	}

	[HttpPut]
	public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto createAccountReq)
	{
		_logger.LogInformation("Creating account");

		if (createAccountReq != null)
		{
			var userId = GetCurrentUserId();

			var account = new Account
			{
				Id = Guid.NewGuid().ToString("N"),
				UserId = userId,
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

			await _repositoryService.CreateAccountAsync(account);

			var apiResponse = new RecordResponseDto<AccountDto>
			{
				IsSuccess = true,
				Result = MapToDto(account)
			};

			return Success(apiResponse);
		}

		return InvalidRequestPayloadError();
	}

	[HttpPost]
	public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDto updateAccountReq)
	{
		_logger.LogInformation("Updating account");

		if (updateAccountReq != null)
		{
			var userId = GetCurrentUserId();

			var account = await _repositoryService.GetAccountByIdAsync(updateAccountReq.Id);
			if (account == null || account.UserId.IsNotEquals(userId))
			{
				return Error("Account does not exist");
			}

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

			await _repositoryService.UpdateAccountAsync(account);

			var apiResponse = new RecordResponseDto<AccountDto>
			{
				IsSuccess = true,
				Result = MapToDto(account)
			};

			return Success(apiResponse);
		}

		return InvalidRequestPayloadError();
	}

	[HttpDelete]
	public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto deleteAccountReq)
	{
		_logger.LogInformation("Deleting account");

		if (deleteAccountReq != null)
		{
			var userId = GetCurrentUserId();

			var account = await _repositoryService.GetAccountByIdAsync(deleteAccountReq.Id);
			if (account == null || account.UserId.IsNotEquals(userId))
			{
				return Error("Account does not exist");
			}

			await _repositoryService.DeleteAccountAsync(deleteAccountReq.Id);
			return Success(new DeletedResponseDto { IsSuccess = true, Result = true });
		}

		return InvalidRequestPayloadError();
	}

	[HttpGet]
	public async Task<IActionResult> GetAccount([FromQuery] string? accountId = null)
	{
		_logger.LogInformation("Getting account(s)");

		var userId = GetCurrentUserId();

		if (accountId.IsNotNullOrWhiteSpace())
		{
			var account = await _repositoryService.GetAccountByIdAsync(accountId);
			if (account == null || account.UserId.IsNotEquals(userId))
			{
				return Error("Account does not exist");
			}

			var apiResponse = new RecordResponseDto<AccountDto>
			{
				IsSuccess = true,
				Result = MapToDto(account)
			};

			return Success(apiResponse);
		}
		else
		{
			var accounts = await _repositoryService.GetAccountsByUserIdAsync(GetCurrentUserId());
			accounts = accounts.OrderBy(x => x.Category).ThenBy(x => x.Name).ToList();
			var apiResponse = new RecordsResponseDto<AccountDto>
			{
				IsSuccess = true,
				Result = accounts.Select(MapToDto).ToList()
			};

			return Success(apiResponse);
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
