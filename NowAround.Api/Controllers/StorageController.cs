using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StorageController(IStorageService storageService) : ControllerBase
{
    
    [Authorize]
    [HttpPost("upload/{type}")]
    public async Task<IActionResult> UploadImageAsync(IFormFile image, [FromRoute] string type, [FromQuery] int? id)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        var role = User.Claims.FirstOrDefault(c => c.Type == "https://now-around-auth-api/roles")?.Value ?? throw new ArgumentException("Role not found");
        var url = await storageService.UploadPictureAsync(image, role, auth0Id, type, id);
        
        return Created(url, "Image successfully uploaded");
    }
}