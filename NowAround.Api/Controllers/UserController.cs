using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Interfaces;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly string _M2MSecretKey;
    
    public UserController(
        IUserService userService, 
        ILogger<UserController> logger, 
        IConfiguration configuration)
    {
        _userService = userService;
        _logger = logger;
        _M2MSecretKey = configuration["Auth0:M2MSecretKey"] ?? throw new ArgumentNullException(configuration["Auth0:M2MSecretKey"]);
    }
    
    /// <summary>
    /// Creates a new userwith the specified Auth0 ID.
    /// Can only be called by the Auth0 server.
    /// </summary>
    /// <param name="auth0Id">The Auth0 ID of the user to be created.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation </returns>
    /// <response code="201"> Returns when the user is successfully created </response>
    /// <response code="401"> Returns when the request is unauthorized </response>
    [HttpPost]
    public async Task<IActionResult> CreateUserAsync(string auth0Id)
    {
        if (!Request.Headers.ContainsKey("Auth0-Server-Token") || 
            Request.Headers["Auth0-Server-Token"] != _M2MSecretKey)
        {
            return Unauthorized("Only the Auth0 server can call this endpoint.");
        }
        
        try
        {
            await _userService.CreateUserAsync(auth0Id);
            return StatusCode(201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating user");
            return BadRequest(e.Message);
        }
    }
}