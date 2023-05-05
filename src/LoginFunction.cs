using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PsGenApi.Document;
using PsGenApi.ServiceModel;
using PsGenApi.Services;

namespace PsGenApi
{
    public class LoginFunction : FunctionBase
    {
        private readonly ILogger _logger;
        private readonly TableService _tableService;

        public LoginFunction(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<LoginFunction>();
            _tableService = new TableService(configuration);
        }

        [Function("login")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Executing login function");

            if (req.Method.ToLowerInvariant() == "post")
            {
                return await HandleLoginAsync(req);
            }

            return await HttpMethodNotSupportedError(req);
        }

        private async Task<HttpResponseData> HandleLoginAsync(HttpRequestData req)
        {
            var loginReq = await ReadRequestBody<LoginRequest>(req);
            if (loginReq != null)
            {
                var userDocument = await _tableService.GetUserDocumentByUsernameAsync(loginReq.Username);
                if (userDocument != null)
                {
                    if (HashService.VerifyHash(loginReq.Password, userDocument.Salt, userDocument.Hash))
                    {
                        var tokenDocument = new TokenDocument
                        {
                            Id = Guid.NewGuid().ToString("N"),
                            UserId = userDocument.Id,
                            Secret = HashService.CreateHash(HashService.CreateSalt(), userDocument.Salt),
                            ExpiresAt = DateTimeOffset.UtcNow.AddDays(90)
                        };

                        await _tableService.CreateOrUpdateTokenDocumentAsync(tokenDocument);

                        var apiResponse = new RecordResponse<UserSession>
                        {
                            IsSuccess = true, Result = new UserSession
                            {
                                Id = tokenDocument.Id,
                                UserId = tokenDocument.UserId,
                                Secret = tokenDocument.Secret,
                                ExpiresAt = tokenDocument.ExpiresAt
                            }
                        };

                        return await Success(req, apiResponse);
                    }
                }

                return await Error(req, "Invalid username or password");
            }

            return await InvalidRequestPayloadError(req);
        }
    }
}
