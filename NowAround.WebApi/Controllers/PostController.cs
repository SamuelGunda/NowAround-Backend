﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Application.Mapping;
using NowAround.Application.Requests;
using NowAround.Application.Services;

namespace NowAround.WebApi.Controllers;

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
    
    [Authorize(Roles = "User")]
    [HttpPut("{postId:int}/react")]
    public async Task<IActionResult> ReactToPostAsync(int postId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");

        await postService.ReactToPostAsync(postId, auth0Id);
        
        return Ok(new { message = "Reacted to post successfully" });
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