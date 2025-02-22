﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Application.Common.Attributes;
using NowAround.Application.Services;

namespace NowAround.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUserAsync(string auth0Id, string fullName)
    {
        await userService.CreateUserAsync(auth0Id, fullName);
        
        return Created("", new { message = "User created successfully" });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUserAsync(string auth0Id)
    {
        var user = await userService.GetUserByAuth0IdAsync(auth0Id);
        
        return Ok(user);
    }
    
    [Authorize(Roles = "User")]
    [HttpPut("picture/{pictureContext}")]
    public async Task<IActionResult> UpdateUserPictureAsync([FromRoute] string pictureContext, [ContentType([ "image/jpeg", "image/png", "image/gif", "image/webp"])] IFormFile picture)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");

        var pictureUrl = await userService.UpdateUserPictureAsync(auth0Id, pictureContext, picture);
        
        return Created("", new { message = "Picture updated successfully" });
    }
    
    [Authorize(Roles = "User")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUserAsync()
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        await userService.DeleteUserAsync(auth0Id);
        
        return NoContent();
    }
}