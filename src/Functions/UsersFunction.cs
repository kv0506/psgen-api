using CSharpExtensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PsGenApi.Document;
using PsGenApi.ServiceModel;
using PsGenApi.Services;

namespace PsGenApi.Functions;

public class UsersFunction(ILoggerFactory loggerFactory, TableService tableService) : FunctionBase
{
	private readonly ILogger _logger = loggerFactory.CreateLogger<UsersFunction>();

	[Function("users")]
	public async Task<HttpResponseData> Run(
		[HttpTrigger(AuthorizationLevel.Anonymous, "post", "put")] HttpRequestData req)
	{
		_logger.LogInformation("Executing users function");

		var httpMethod = req.Method.ToLowerInvariant();

		if (httpMethod == "put") return await HandleCreateUserAsync(req);

		if (httpMethod == "post") return await HandleUpdateUserAsync(req);

		return await HttpMethodNotSupportedError(req);
	}

	private async Task<HttpResponseData> HandleCreateUserAsync(HttpRequestData req)
	{
		var createUserReq = await ReadRequestBody<CreateUserDto>(req);
		if (createUserReq != null)
		{
			var user = await tableService.GetUserDocumentByUsernameAsync(createUserReq.Username);
			if (user == null)
			{
				user = new User
				{
					Id = Guid.NewGuid().ToString("N"),
					Username = createUserReq.Username,
					Email = createUserReq.Email,
					Mobile = createUserReq.Mobile,
					Salt = HashService.CreateSalt()
				};

				user.Hash = HashService.CreateHash(createUserReq.Password, user.Salt);

				await tableService.CreateOrUpdateUserDocumentAsync(user);

				var apiResponse = new RecordResponseDto<UserDto>
				{
					IsSuccess = true,
					Result = new UserDto
					{
						Id = user.Id,
						Username = user.Username,
						Email = user.Email,
						Mobile = user.Mobile
					}
				};

				return await Success(req, apiResponse);
			}

			return await Error(req, "Username already exists");
		}

		return await InvalidRequestPayloadError(req);
	}

	private async Task<HttpResponseData> HandleUpdateUserAsync(HttpRequestData req)
	{
		var updateUserReq = await ReadRequestBody<UpdateUserDto>(req);
		if (updateUserReq != null)
		{
			var user = await tableService.GetUserDocumentByUserIdAsync(updateUserReq.Id);
			if (user != null)
			{
				if (updateUserReq.NewPassword.IsNotNullOrWhiteSpace())
				{
					if (!HashService.VerifyHash(updateUserReq.CurrentPassword, user.Salt, user.Hash))
						return await Error(req, "Current password is invalid");

					user.Hash = HashService.CreateHash(updateUserReq.NewPassword, user.Salt);
				}

				if (updateUserReq.Email.IsNotNullOrWhiteSpace()) user.Email = updateUserReq.Email;

				if (updateUserReq.Mobile.IsNotNullOrWhiteSpace()) user.Mobile = updateUserReq.Mobile;

				await tableService.CreateOrUpdateUserDocumentAsync(user);

				var apiResponse = new RecordResponseDto<UserDto>
				{
					IsSuccess = true,
					Result = new UserDto
					{
						Id = user.Id,
						Username = user.Username,
						Email = user.Email,
						Mobile = user.Mobile
					}
				};

				return await Success(req, apiResponse);
			}

			return await Error(req, "User does not exist");
		}

		return await InvalidRequestPayloadError(req);
	}
}