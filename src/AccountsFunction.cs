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
    public class AccountsFunction : FunctionBase
    {
        private readonly ILogger _logger;
        private readonly TableService _tableService;

        public AccountsFunction(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<UsersFunction>();
            _tableService = new TableService(configuration);
        }

        [Function("accounts")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete")] HttpRequestData req)
        {
            _logger.LogInformation("Executing accounts function");

            if (!req.Headers.Contains("AuthToken"))
            {
                return await AuthError(req, "AuthToken header is missing");
            }

            var token = req.Headers.GetValues("AuthToken").FirstOrDefault();
            if (token.IsNullOrWhiteSpace())
            {
                return await AuthError(req, "AuthToken is empty");
            }

            var tokenDocument = await _tableService.GetTokenDocumentAsync(token);
            if (tokenDocument == null || tokenDocument.ExpiresAt < DateTimeOffset.UtcNow)
            {
                return await AuthError(req, "Invalid AuthToken");
            }

            var httpMethod = req.Method.ToLowerInvariant();

            if (httpMethod == "put")
            {
                return await HandleCreateAccountAsync(req, tokenDocument);
            }

            if (httpMethod == "post")
            {
                return await HandleUpdateAccountAsync(req, tokenDocument);
            }

            if (httpMethod == "delete")
            {
                return await HandleDeleteAccountAsync(req, tokenDocument);
            }

            if (httpMethod == "get")
            {
                return await HandleGetAccountAsync(req, tokenDocument);
            }
             
            return await HttpMethodNotSupportedError(req);
        }

        private async Task<HttpResponseData> HandleCreateAccountAsync(HttpRequestData req, TokenDocument tokenDocument)
        {
            var createAccountReq = await ReadRequestBody<CreateAccount>(req);
            if (createAccountReq != null)
            {
                var accountDocument = new AccountDocument
                {
                    Id = Guid.NewGuid().ToString("N"),
                    UserId = tokenDocument.UserId,
                    Name = createAccountReq.Name,
                    Category = createAccountReq.Category,
                    Pattern = createAccountReq.Pattern,
                    Length = createAccountReq.Length,
                    IncludeSpecialCharacter = createAccountReq.IncludeSpecialCharacter,
                    UseCustomSpecialCharacter = createAccountReq.UseCustomSpecialCharacter,
                    CustomSpecialCharacter = createAccountReq.CustomSpecialCharacter,
                };

                await _tableService.CreateOrUpdateAccountDocumentAsync(tokenDocument.UserId, accountDocument);

                var apiResponse = new RecordResponse<Account>
                {
                    IsSuccess = true,
                    Result = Map(accountDocument)
                };

                return await Success(req, apiResponse);
            }

            return await InvalidRequestPayloadError(req);
        }

        private async Task<HttpResponseData> HandleUpdateAccountAsync(HttpRequestData req, TokenDocument tokenDocument)
        {
            var updateAccountReq = await ReadRequestBody<UpdateAccount>(req);
            if (updateAccountReq != null)
            {
                var accountDocument = await _tableService.GetAccountDocumentAsync(updateAccountReq.Id);
                if (accountDocument == null || accountDocument.UserId.IsNotEquals(tokenDocument.UserId))
                {
                    return await Error(req, "Account does not exist");
                }

                accountDocument.Name = updateAccountReq.Name;
                accountDocument.Category = updateAccountReq.Category;
                accountDocument.Pattern = updateAccountReq.Pattern;
                accountDocument.Length = updateAccountReq.Length;
                accountDocument.IncludeSpecialCharacter = updateAccountReq.IncludeSpecialCharacter;
                accountDocument.UseCustomSpecialCharacter = updateAccountReq.UseCustomSpecialCharacter;
                accountDocument.CustomSpecialCharacter = updateAccountReq.CustomSpecialCharacter;

                await _tableService.CreateOrUpdateAccountDocumentAsync(tokenDocument.UserId, accountDocument);

                var apiResponse = new RecordResponse<Account>
                {
                    IsSuccess = true,
                    Result = Map(accountDocument)
                };

                return await Success(req, apiResponse);
            }

            return await InvalidRequestPayloadError(req);
        }

        private async Task<HttpResponseData> HandleDeleteAccountAsync(HttpRequestData req, TokenDocument tokenDocument)
        {
            var deleteAccountReq = await ReadRequestBody<DeleteAccount>(req);
            if (deleteAccountReq != null)
            {
                var accountDocument = await _tableService.GetAccountDocumentAsync(deleteAccountReq.Id);
                if (accountDocument == null || accountDocument.UserId.IsNotEquals(tokenDocument.UserId))
                {
                    return await Error(req, "Account does not exist");
                }

                await _tableService.DeleteAccountDocumentAsync(deleteAccountReq.Id);
                return await Success(req, new DeletedResponse());
            }

            return await InvalidRequestPayloadError(req);
        }

        private async Task<HttpResponseData> HandleGetAccountAsync(HttpRequestData req, TokenDocument tokenDocument)
        {
            var accountId = req.Query.Get("accountId");
            if (accountId.IsNotNullOrWhiteSpace())
            {
                var accountDocument = await _tableService.GetAccountDocumentAsync(accountId);
                if (accountDocument == null || accountDocument.UserId.IsNotEquals(tokenDocument.UserId))
                {
                    return await Error(req, "Account does not exist");
                }

                var apiResponse = new RecordResponse<Account>
                {
                    IsSuccess = true,
                    Result = Map(accountDocument)
                };

                return await Success(req, apiResponse);
            }
            else
            {
                var documents = await _tableService.GetAccountDocumentsAsync(tokenDocument.UserId);
                var apiResponse = new RecordsResponse<Account>
                {
                    IsSuccess = true,
                    Result = documents.Select(Map).ToList()
                };

                return await Success(req, apiResponse);
            }
        }

        private Account Map(AccountDocument document)
        {
            return new Account
            {
                Id = document.Id,
                Name = document.Name,
                Category = document.Category,
                Pattern = document.Pattern,
                Length = document.Length,
                IncludeSpecialCharacter = document.IncludeSpecialCharacter,
                UseCustomSpecialCharacter = document.UseCustomSpecialCharacter,
                CustomSpecialCharacter = document.CustomSpecialCharacter
            };
        }
    }
}
