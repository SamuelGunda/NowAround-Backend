using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NowAround.WebApi.Controllers;

[ExcludeFromCodeCoverage]
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