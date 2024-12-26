using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Models.Requests;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController(IPostService postService) : ControllerBase
{

    [HttpPost]
    [Authorize (Roles = "Establishment")]
    public async Task<IActionResult> CreatePostAsync([FromBody] PostCreateRequest postCreateRequest)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        var postId = await postService.CreatePostAsync(postCreateRequest, auth0Id);
        
        return Created("", new { message = $"Post created successfully under id: {postId}" });
    }
}