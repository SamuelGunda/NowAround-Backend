using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Interfaces;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstablishmentController(IEstablishmentService establishmentService, ILogger<EstablishmentController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateEstablishmentAsync(EstablishmentRegisterRequest establishment)
    {
        try
        {
            await establishmentService.RegisterEstablishmentAsync(establishment);
            return StatusCode(201);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating establishment");
            throw;
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEstablishmentByAuth0IdAsync(string id)
    {
        try
        {
            /*EstablishmentDto establishment;
        
            if (int.TryParse(id , out var numericId))
            {
                establishment = await establishmentService.GetEstablishmentByIdAsync(numericId);
            }
            else
            {
                establishment = await establishmentService.GetEstablishmentByAuth0IdAsync(id);
            }*/
            
            var establishment = await establishmentService.GetEstablishmentByAuth0IdAsync(id);
            
            return Ok(establishment);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting establishment");
            throw;
        }
    }
    
    //TODO: Implement search
    /*[HttpGet("search")]
    public async Task<IActionResult> SearchEstablishmentsAsync(string? name, string? categoryName, List<string>? tagNames)
    {
        var tags = tagNames != null ? string.Join(", ", tagNames) : "No tags";
        return Ok(new { message = $"name: {name}, categoryName: {categoryName}, tagNames: {tags}, tag count: {tagNames[0]}" });
    }*/

    
    [HttpGet("search-pins")]
    public async Task<IActionResult> GetEstablishmentMarkersWithFilterInAreaAsync(
        double northWestLat, double northWestLong, 
        double southEastLat, double southEastLong, 
        string? name, string? categoryName, List<string>? tagNames)
    {
        
        var mapBounds = new MapBounds
        {
            NwLat = northWestLat,
            NwLong = northWestLong,
            SeLat = southEastLat,
            SeLong = southEastLong
        };
        
        try
        {
            var establishmentMarker = await establishmentService.GetEstablishmentMarkersWithFilterInAreaAsync(mapBounds, name, categoryName, tagNames);
            if (establishmentMarker == null)
            {
                return NotFound(new { message = "No markers found for the specified location" });
            }
            return Ok(establishmentMarker);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting establishments in area");
            throw;
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateEstablishmentAsync(EstablishmentUpdateRequest establishmentUpdateRequest)
    {
        try
        {
            await establishmentService.UpdateEstablishmentAsync(establishmentUpdateRequest);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating establishment");
            throw;
        }
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteEstablishmentAsync(string auth0Id)
    {
        try
        {
            await establishmentService.DeleteEstablishmentAsync(auth0Id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting establishment");
            throw;
        }
    }
    
    // Admin only calls
    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingEstablishmentsAsync()
    {
        try
        {
            var establishments = await establishmentService.GetPendingEstablishmentsAsync();
            return Ok(establishments);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting pending establishments");
            throw;
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("register-status")]
    public async Task<IActionResult> UpdateEstablishmentRegisterRequestAsync(string auth0Id, string action)
    {
        Console.WriteLine("Action");
        try
        {
            if (action.Equals("accept", StringComparison.OrdinalIgnoreCase))
            {
                await establishmentService.UpdateEstablishmentRegisterRequestAsync(auth0Id, RequestStatus.Accepted);
            }
            else if (action.Equals("reject", StringComparison.OrdinalIgnoreCase))
            {
                await establishmentService.UpdateEstablishmentRegisterRequestAsync(auth0Id, RequestStatus.Rejected);
            }
            else
            {
                return BadRequest("Invalid action type");
            }

            return Ok();
            
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error accepting establishment");
            throw;
        }
    }
}