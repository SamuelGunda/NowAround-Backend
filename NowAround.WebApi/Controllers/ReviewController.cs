using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Application.Requests;
using NowAround.Application.Services;

namespace NowAround.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> CreateReviewAsync(ReviewCreateRequest reviewCreateRequest)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");    
        
        var reviewDto = await reviewService.CreateReviewAsync(auth0Id, reviewCreateRequest);
        
        return Created("", reviewDto);
    }
}