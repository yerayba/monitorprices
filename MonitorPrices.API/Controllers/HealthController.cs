using Microsoft.AspNetCore.Mvc;

namespace MonitorPrices.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "Ping")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            status = "OK",
            message = "Pong..!",
            timestamp = DateTime.UtcNow
        });
    }
}