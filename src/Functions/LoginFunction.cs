using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PsGenApi.Document;
using PsGenApi.ServiceModel;
using PsGenApi.Services;

namespace PsGenApi.Functions;

public class LoginFunction(ILoggerFactory loggerFactory, TableService tableService) : FunctionBase
{
	private readonly ILogger _logger = loggerFactory.CreateLogger<LoginFunction>();

	[Function("login")]
	public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
	{
		_logger.LogInformation("Executing login function");

		if (req.Method.ToLowerInvariant() == "post") return await HandleLoginAsync(req);

		return await HttpMethodNotSupportedError(req);
	}

	private async Task<HttpResponseData> HandleLoginAsync(HttpRequestData req)
	{
		var loginReq = await ReadRequestBody<LoginRequestDto>(req);
		if (loginReq != null)
		{
			var user = await tableService.GetUserDocumentByUsernameAsync(loginReq.Username);

			if (user != null && HashService.VerifyHash(loginReq.Password, user.Salt, user.Hash))
			{
				var token = new Token
				{
					Id = Guid.NewGuid().ToString("N"),
					UserId = user.Id,
					Secret = HashService.CreateHash(HashService.CreateSalt(), user.Salt),
					ExpiresAt = DateTimeOffset.UtcNow.AddDays(90)
				};

				await tableService.CreateOrUpdateTokenDocumentAsync(token);

				var apiResponse = new RecordResponseDto<UserSessionDto>
				{
					IsSuccess = true,
					Result = new UserSessionDto
					{
						Id = token.Id,
						UserId = token.UserId,
						Secret = token.Secret,
						ExpiresAt = token.ExpiresAt
					}
				};

				return await Success(req, apiResponse);
			}

			return await Error(req, "Invalid username or password");
		}

		return await InvalidRequestPayloadError(req);
	}
}