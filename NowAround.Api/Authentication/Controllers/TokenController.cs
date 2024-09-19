using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Authentication.Interfaces;

namespace NowAround.Api.Authentication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController(ITokenService tokenService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetManagementToken()
    {
        try
        {
            var token = await tokenService.GetManagementAccessTokenAsync();
            return Ok(token);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}