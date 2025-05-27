using Microsoft.AspNetCore.Mvc;

namespace PsGenApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Check()
    {
        _logger.LogInformation("Health check executed at {Time}", DateTime.UtcNow);
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }
}
