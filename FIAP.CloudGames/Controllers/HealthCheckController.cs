using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.API.Controllers;

[ApiController]
public class HealthCheckController : Controller
{
    [HttpGet("/health")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            application = "FIAP CloudGames",
            apiVersion = "1.0.0"
        });
    }
}
