﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Services.Interfaces;
using NowAround.Api.Utilities;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstablishmentController(
    IEstablishmentService establishmentService,
    ILogger<EstablishmentController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishment)
    {
        await establishmentService.RegisterEstablishmentAsync(establishment);
        
        return Created("", new { message = "Establishment created successfully" });
    }
    
    [HttpGet("profile/{auth0Id}")]
    public async Task<IActionResult> GetEstablishmentProfileInfoByAuth0IdAsync(string auth0Id)
    {
        var establishment = await establishmentService.GetEstablishmentProfileByAuth0IdAsync(auth0Id);
        
        return Ok(establishment);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingEstablishmentsAsync()
    {
        var establishments = await establishmentService.GetPendingEstablishmentsAsync();
        if (establishments.Count == 0)
        {
            return NoContent();
        }
        
        return Ok(establishments);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> GetEstablishmentsWithFilterAsync(
        double? northWestLat, double? northWestLong, 
        double? southEastLat, double? southEastLong, 
        string? name, int? priceCategory, 
        string? categoryName, string? tagNames, int? page)
    {
        var tagNamesList = tagNames?.Split(',').ToList();
        var searchValues = new SearchValues
        {
            Name = name,
            PriceCategory = priceCategory,
            CategoryName = categoryName,
            TagNames = tagNamesList,
            
            MapBounds = new MapBounds
            {
                NwLat = northWestLat ?? 0,
                NwLong = northWestLong ?? 0,
                SeLat = southEastLat ?? 0,
                SeLong = southEastLong ?? 0
            }
        };
        
        var establishmentDtos = await establishmentService.GetEstablishmentsWithFilterAsync(searchValues, page ?? 0);
        if (establishmentDtos.Count == 0)
        {
            return NoContent();
        }
        
        return Ok(establishmentDtos);
    }
    
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateEstablishmentAsync(EstablishmentUpdateRequest establishmentUpdateRequest)
    {
        if (AuthorizationHelper.HasAdminRightsOrMatchingAuth0Id(User, establishmentUpdateRequest.Auth0Id))
        {
            await establishmentService.UpdateEstablishmentAsync(establishmentUpdateRequest);
            return NoContent();
        }

        return Forbid();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("register-status")]
    public async Task<IActionResult> UpdateEstablishmentRegisterRequestAsync(string auth0Id, string action)
    {
        if (Enum.TryParse<RequestStatus>(action + "ed", true, out var status) && 
            status is RequestStatus.Accepted or RequestStatus.Rejected)
        {
            await establishmentService.UpdateEstablishmentRegisterRequestAsync(auth0Id, status);
            return NoContent();
        }

        logger.LogWarning("Invalid action type provided: {Action}", action);
        return BadRequest(new { message = "Invalid action type. Please use 'accept' or 'reject'." });
    }
    
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteEstablishmentAsync(string auth0Id)
    {
        if (AuthorizationHelper.HasAdminRightsOrMatchingAuth0Id(User, auth0Id))
        {
            await establishmentService.DeleteEstablishmentAsync(auth0Id);
            return NoContent();
        }

        return Forbid();
    }
}