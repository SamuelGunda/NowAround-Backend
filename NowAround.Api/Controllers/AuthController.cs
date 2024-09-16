using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Interfaces;
using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController :ControllerBase
{
    
    private readonly IAuth0Service _auth0Service;

    public AuthController(IAuth0Service auth0Service)
    {
        _auth0Service = auth0Service;
    }
    
    [HttpPost ("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            
            var userId = await _auth0Service.RegisterUserAsync(registerUserDto);
            return Ok(new { UserId = userId });
            
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost ("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var token = await _auth0Service.LoginUserAsync(loginUserDto);
            return Ok(new { Token = token });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}