using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Apis.Auth0.Models;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Interfaces;
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
            var establishmentId = await establishmentService.RegisterEstablishmentAsync(establishment);
            return StatusCode(201);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating establishment");
            throw;
        }
    }
    
    [HttpGet("id")]
    public async Task<IActionResult> GetEstablishmentByIdAsync(int id)
    {
        try
        {
            var establishment = await establishmentService.GetEstablishmentByIdAsync(id);
            return Ok(establishment);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting establishment");
            throw;
        }
    }

    [HttpGet("auth0-id")]
    public async Task<IActionResult> GetEstablishmentAsync(string auth0Id)
    {
        try
        {
            var establishment = await establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id);
            return Ok(establishment);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting establishment");
            throw;
        }
    }
    
    [HttpPost("area-pins")]
    public async Task<IActionResult> GetEstablishmentPinsInAreaAsync(EstablishmentsInAreaRequest establishmentsInAreaRequest)
    {
        try
        {
            var establishmentPins = await establishmentService.GetEstablishmentPinsByAreaAsync(establishmentsInAreaRequest);
            if (establishmentPins == null)
            {
                return NotFound(new { message = "No pins found for the specified location" });
            }
            return Ok(establishmentPins);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting establishments in area");
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
}