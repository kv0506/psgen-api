using Microsoft.AspNetCore.Mvc;
using PsGenApi.Document;
using PsGenApi.ServiceModel;
using PsGenApi.Services;

namespace PsGenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController(ILogger<LoginController> logger, IRepositoryService repositoryService)
	: BaseController
{
	[HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginReq)
    {
        logger.LogInformation("Processing login request");

        if (loginReq != null)
        {
            var user = await repositoryService.GetUserByUsernameAsync(loginReq.Username);

            if (user != null && HashService.VerifyHash(loginReq.Password, user.Salt, user.Hash))
            {
                var token = new Token
                {
                    Id = Guid.NewGuid().ToString("N"),
                    UserId = user.Id,
                    Secret = HashService.CreateHash(HashService.CreateSalt(), user.Salt),
                    ExpiresAt = DateTimeOffset.UtcNow.AddDays(90)
                };

                await repositoryService.CreateTokenAsync(token);

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

                return Success(apiResponse);
            }

            return Error("Invalid username or password");
        }

        return InvalidRequestPayloadError();
    }
}
