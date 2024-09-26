using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Authentication.Interfaces;

namespace NowAround.Api.Authentication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUserAsync(string auth0Id)
    {
        
        if (string.IsNullOrEmpty(auth0Id))
        {
            return BadRequest("Auth0Id is required");
        }
        
        try
        {
            await userService.CreateUserAsync(auth0Id);
            return StatusCode(201);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
 
}