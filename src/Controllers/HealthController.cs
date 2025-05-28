using Microsoft.AspNetCore.Mvc;

namespace PsGenApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController(ILogger<HealthController> logger) : ControllerBase
{
	[HttpGet]
    public IActionResult Check()
    {
        logger.LogInformation("Health check executed at {Time}", DateTime.UtcNow);
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }
}
