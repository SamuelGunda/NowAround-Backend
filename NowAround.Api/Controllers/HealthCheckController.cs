using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    
    [HttpGet("exception")]
    public IActionResult TestException()
    {
        throw new InvalidOperationException("Test exception to verify global handler.");
    }
    
    /*
    [HttpGet("establishment")]
    [Authorize(Roles = "Establishment")]
    public Task<IActionResult> EstablishmentAuthorization()
    {
        return Task.FromResult<IActionResult>(Ok("Hello Establishment"));
    }
    
    [HttpGet("user")]
    [Authorize(Roles = "User")]
    public Task<IActionResult> UserAuthorization()
    {
        return Task.FromResult<IActionResult>(Ok("Hello User"));
    }
    
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public Task<IActionResult> AdminAuthorization()
    {
        return Task.FromResult<IActionResult>(Ok("Hello Admin"));
    }
    */
}