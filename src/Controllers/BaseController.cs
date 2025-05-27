using System.Net;
using CSharpExtensions;
using Microsoft.AspNetCore.Mvc;
using PsGenApi.ServiceModel;
using PsGenApi.Services;

namespace PsGenApi.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult Success<T>(T apiResponse)
    {
        return Ok(apiResponse);
    }

    protected IActionResult HttpMethodNotSupportedError()
    {
        return BadRequest(new ApiResponseDto { IsSuccess = false, Message = "Http Method not supported" });
    }

    protected IActionResult InvalidRequestPayloadError()
    {
        return BadRequest(new ApiResponseDto { IsSuccess = false, Message = "Invalid request payload" });
    }

    protected IActionResult Error(string error)
    {
        return BadRequest(new ApiResponseDto { IsSuccess = false, Message = error });
    }

    protected IActionResult AuthError(string error)
    {
        return Unauthorized(new ApiResponseDto { IsSuccess = false, Message = error });
    }

    protected string GetCurrentUserId()
	{
		return User.FindFirst("sub")?.Value ?? string.Empty;
	}
}
