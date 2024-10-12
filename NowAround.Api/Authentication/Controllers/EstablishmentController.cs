﻿using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Authentication.Models;

namespace NowAround.Api.Authentication.Controllers;

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
    
    [HttpGet]
    public async Task<IActionResult> GetEstablishmentAsync(string auth0Id)
    {
        try
        {
            var establishment = await establishmentService.GetEstablishmentAsync(auth0Id);
            return Ok(establishment);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting establishment");
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