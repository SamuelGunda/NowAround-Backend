using Microsoft.AspNetCore.Mvc;

namespace NowAround.Api.Authentication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> CheckIfAccountExist(string auth0Id)
    {
        try
        {
            Console.WriteLine(auth0Id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
 
}