using CSharpExtensions;
using Microsoft.AspNetCore.Mvc;
using PsGenApi.Document;
using PsGenApi.ServiceModel;
using PsGenApi.Services;

namespace PsGenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly ILogger<UsersController> _logger;
    private readonly IRepositoryService _repositoryService;

    public UsersController(ILogger<UsersController> logger, IRepositoryService repositoryService)
    {
        _logger = logger;
        _repositoryService = repositoryService;
    }

    [HttpPut]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserReq)
    {
        _logger.LogInformation("Creating user");

        if (createUserReq != null)
        {
            var user = await _repositoryService.GetUserByUsernameAsync(createUserReq.Username);
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

                await _repositoryService.CreateUserAsync(user);

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

                return Success(apiResponse);
            }

            return Error("Username already exists");
        }

        return InvalidRequestPayloadError();
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserReq)
    {
        _logger.LogInformation("Updating user");

        if (updateUserReq != null)
        {
            var user = await _repositoryService.GetUserByIdAsync(updateUserReq.Id);
            if (user != null)
            {
                if (updateUserReq.NewPassword.IsNotNullOrWhiteSpace())
                {
                    if (!HashService.VerifyHash(updateUserReq.CurrentPassword, user.Salt, user.Hash))
                    {
                        return Error("Current password is invalid");
                    }

                    user.Hash = HashService.CreateHash(updateUserReq.NewPassword, user.Salt);
                }

                if (updateUserReq.Email.IsNotNullOrWhiteSpace())
                {
                    user.Email = updateUserReq.Email;
                }

                if (updateUserReq.Mobile.IsNotNullOrWhiteSpace())
                {
                    user.Mobile = updateUserReq.Mobile;
                }

                await _repositoryService.UpdateUserAsync(user);

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

                return Success(apiResponse);
            }

            return Error("User does not exist");
        }

        return InvalidRequestPayloadError();
    }
}
