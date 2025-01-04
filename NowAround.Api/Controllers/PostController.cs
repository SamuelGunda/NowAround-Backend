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

    [Authorize(Roles = "Establishment")]
    [HttpPost]
    public async Task<IActionResult> CreatePostAsync([FromBody] PostCreateRequest postCreateRequest)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");

        var postId = await postService.CreatePostAsync(postCreateRequest, auth0Id);
        
        return Created("", new { message = $"Post created successfully under id: {postId}" });
    }
    
    [HttpGet("{postId:int}")]
    public async Task<IActionResult> GetPostAsync(int postId)
    {
        var post = await postService.GetPostAsync(postId);
        
        return Ok(post.ToDto());
    }
    
    [Authorize(Roles = "Establishment")]
    [HttpDelete("{postId:int}")]
    public async Task<IActionResult> DeletePostAsync(int postId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        var isOwner = await postService.CheckPostOwnershipByAuth0IdAsync(auth0Id, postId);
        
        if (!isOwner)
        {
            return Forbid();
        }
        
        await postService.DeletePostAsync(postId);
        
        return Ok(new { message = "Post deleted successfully" });
    }
}