using CSharpExtensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PsGenApi.Document;
using PsGenApi.ServiceModel;
using PsGenApi.Services;

namespace PsGenApi
{
    public class UsersFunction : FunctionBase
    {
        private readonly ILogger _logger;
        private readonly TableService _tableService;

        public UsersFunction(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<UsersFunction>();
            _tableService = new TableService(configuration);
        }

        [Function("users")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", "put")] HttpRequestData req)
        {
            _logger.LogInformation("Executing users function");

            var httpMethod = req.Method.ToLowerInvariant();

            if (httpMethod == "put")
            {
                return await HandleCreateUserAsync(req);
            }
            
            if (httpMethod == "post")
            {
                return await HandleUpdateUserAsync(req);
            }

            return await HttpMethodNotSupportedError(req);
        }

        private async Task<HttpResponseData> HandleCreateUserAsync(HttpRequestData req)
        {
            var createUserReq = await ReadRequestBody<CreateUser>(req);
            if (createUserReq != null)
            {
                var document = await _tableService.GetUserDocumentByUsernameAsync(createUserReq.Username);
                if (document == null)
                {
                    document = new UserDocument
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Username = createUserReq.Username,
                        Email = createUserReq.Email,
                        Mobile = createUserReq.Mobile,
                        Salt = HashService.CreateSalt()
                    };

                    document.Hash = HashService.CreateHash(createUserReq.Password, document.Salt);

                    await _tableService.CreateOrUpdateUserDocumentAsync(document);

                    var apiResponse = new RecordResponse<User>
                    {
                        IsSuccess = true,
                        Result = new User
                        {
                            Id = document.Id,
                            Username = document.Username,
                            Email = document.Email,
                            Mobile = document.Mobile
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
            var updateUserReq = await ReadRequestBody<UpdateUser>(req);
            if (updateUserReq != null)
            {
                var document = await _tableService.GetUserDocumentByUserIdAsync(updateUserReq.Id);
                if (document != null)
                {
                    if (updateUserReq.NewPassword.IsNotNullOrWhiteSpace())
                    {
                        if (!HashService.VerifyHash(updateUserReq.CurrentPassword, document.Salt, document.Hash))
                        {
                            return await Error(req, "Current password is invalid");
                        }

                        document.Hash = HashService.CreateHash(updateUserReq.NewPassword, document.Salt);
                    }

                    if (updateUserReq.Email.IsNotNullOrWhiteSpace())
                    {
                        document.Email = updateUserReq.Email;
                    }

                    if (updateUserReq.Mobile.IsNotNullOrWhiteSpace())
                    {
                        document.Mobile = updateUserReq.Mobile;
                    }

                    await _tableService.CreateOrUpdateUserDocumentAsync(document);

                    var apiResponse = new RecordResponse<User>
                    {
                        IsSuccess = true,
                        Result = new User
                        {
                            Id = document.Id,
                            Username = document.Username,
                            Email = document.Email,
                            Mobile = document.Mobile
                        }
                    };

                    return await Success(req, apiResponse);
                }

                return await Error(req, "User does not exist");
            }

            return await InvalidRequestPayloadError(req);
        }
    }
}
