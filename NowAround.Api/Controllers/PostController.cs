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
    public async Task<IActionResult> CreatePostAsync([FromForm] PostCreateUpdateRequest postCreateRequest)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");

        var postDto = await postService.CreatePostAsync(postCreateRequest, auth0Id);
        
        return Created("", postDto);
    }
    
    [HttpGet("{postId:int}")]
    public async Task<IActionResult> GetPostAsync(int postId)
    {
        var post = await postService.GetPostAsync(postId);
        
        return Ok(post.ToDto());
    }
    
    [Authorize(Roles = "Establishment")]
    [HttpPut("{postId:int}")]
    public async Task<IActionResult> UpdatePostAsync(int postId, [FromForm] PostCreateUpdateRequest postUpdateRequest)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");

        var postDto = await postService.UpdatePostAsync(postId, auth0Id, postUpdateRequest);
        
        return Ok(postDto);
    }
    
    [Authorize(Roles = "Establishment")]
    [HttpDelete("{postId:int}")]
    public async Task<IActionResult> DeletePostAsync(int postId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        await postService.DeletePostAsync(auth0Id, postId);
        
        return Ok(new { message = "Post deleted successfully" });
    }
}