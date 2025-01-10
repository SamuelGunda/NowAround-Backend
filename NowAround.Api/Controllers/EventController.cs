using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Models.Requests;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Controllers;

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
}