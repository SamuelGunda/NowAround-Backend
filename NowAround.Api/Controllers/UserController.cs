using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Interfaces;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUserAsync(string auth0Id)
    {
        try
        {
            await userService.CreateUserAsync(auth0Id);
            return StatusCode(201);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating user");
            return BadRequest(e.Message);
        }
    }
 
}