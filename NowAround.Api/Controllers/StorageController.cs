using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Services.Interfaces;
using NowAround.Api.Utilities;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StorageController(IStorageService storageService) : ControllerBase
{
    
    [HttpPost("upload/{auth0Id}/{type}/{id?}")]
    [Authorize]
    public async Task<IActionResult> UploadImageAsync(IFormFile image, [FromRoute] string auth0Id, [FromRoute] string type, [FromRoute] string? id = null)
    {
        if (AuthorizationHelper.HasAdminRightsOrMatchingAuth0Id(User, auth0Id))
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == "https://now-around-auth-api/roles")?.Value;
            var url = await storageService.UploadImageAsync(image, role, auth0Id, type, id);
            
            return Created(url, "Image successfully uploaded");
        }
        return Unauthorized();
    }
}