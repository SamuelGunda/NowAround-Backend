using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Application.Requests;
using NowAround.Application.Services;

namespace NowAround.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController(IEventService eventService) : ControllerBase
{
    [Authorize(Roles = "Establishment")]
    [HttpPost]
    public async Task<IActionResult> CreateEventAsync([FromForm] EventCreateRequest eventCreateRequest)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");

        var eventDto = await eventService.CreateEventAsync(auth0Id, eventCreateRequest);
        
        return Created("", eventDto);
    }
    
    [Authorize(Roles = "User")]
    [HttpPut("{eventId:int}/react")]
    public async Task<IActionResult> ReactToEventAsync(int eventId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");

        await eventService.ReactToEventAsync(eventId, auth0Id);
        
        return Ok(new { message = "Reacted to event successfully" });
    }
    
    
    [Authorize(Roles = "Establishment")]
    [HttpDelete("{eventId:int}")]
    public async Task<IActionResult> DeleteEventAsync(int eventId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        await eventService.DeleteEventAsync(auth0Id, eventId);
        
        return NoContent();
    }
}