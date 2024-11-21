using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Services;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    
    public UserController(
        IUserService userService, 
        ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync(string auth0Id, string fullName)
    {
        await _userService.CreateUserAsync(auth0Id, fullName);
        return Created("", new { message = "User created successfully" });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUserAsync(string auth0Id)
    {
        var user = await _userService.GetUserAsync(auth0Id);
        return Ok(user);
    }
}