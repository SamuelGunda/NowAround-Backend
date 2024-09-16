using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NowAround.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Healthy");
    }
    
    [Authorize]
    [HttpGet("secure")]
    public IActionResult GetSecure()
    {
        return Ok("Secure");
    }
}