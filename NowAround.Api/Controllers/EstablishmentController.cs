using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Services.Interfaces;
using NowAround.Api.Utilities;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstablishmentController(IEstablishmentService establishmentService) : ControllerBase
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
    
    [Authorize(Roles = "Establishment")]
    [HttpPut("generic-info")]
    public async Task<IActionResult> UpdateEstablishmentGenericInfoAsync(EstablishmentGenericInfoUpdateRequest establishmentGenericInfoUpdateRequest)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                      ?? throw new ArgumentException("Auth0Id not found");
        
        var genericInfo = await establishmentService.UpdateEstablishmentGenericInfoAsync(auth0Id, establishmentGenericInfoUpdateRequest);
        
        return Ok(genericInfo);
    }
    
    [Authorize(Roles = "Establishment")]
    [HttpPut("location-info")]
    public async Task<IActionResult> UpdateEstablishmentLocationInfoAsync(EstablishmentLocationInfoUpdateRequest establishmentLocationInfoUpdateRequest)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                      ?? throw new ArgumentException("Auth0Id not found");
        
        var locationInfo = await establishmentService.UpdateEstablishmentLocationInfoAsync(auth0Id, establishmentLocationInfoUpdateRequest);
        
        return Ok(locationInfo);
    }
    
    [Authorize(Roles = "Establishment")]
    [HttpPut ("picture/{pictureContext}")]
    public async Task<IActionResult> UpdateEstablishmentPictureAsync([FromRoute] string pictureContext, [ContentType([ "image/jpeg", "image/png", "image/gif", "image/webp"])] IFormFile picture)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                      ?? throw new ArgumentException("Auth0Id not found");
        
        var pictureUrl = await establishmentService.UpdateEstablishmentPictureAsync(auth0Id, pictureContext, picture);
        
        return Ok(new { pictureUrl });
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
        
        return BadRequest(new { message = "Invalid action type. Please use 'accept' or 'reject'." });
    }
    
    [Authorize(Roles = "Establishment")]
    [HttpDelete]
    public async Task<IActionResult> DeleteEstablishmentAsync()
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                      ?? throw new ArgumentException("Auth0Id not found");
        
        await establishmentService.DeleteEstablishmentAsync(auth0Id);

        return NoContent();
    }
    
    //Menu Endpoints
    
    [Authorize(Roles = "Establishment")]
    [HttpPost("menu")]
    public async Task<IActionResult> AddMenuAsync(MenuCreateRequest menu)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                      ?? throw new ArgumentException("Auth0Id not found");
        
        var menuDto = await establishmentService.AddMenuAsync(auth0Id, menu);
        
        return Created("", menuDto);
    }

    [Authorize(Roles = "Establishment")]
    [HttpPut("menu")]
    public async Task<IActionResult> UpdateMenuAsync(MenuUpdateRequest menu)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                      ?? throw new ArgumentException("Auth0Id not found");
        
        var menuDto = await establishmentService.UpdateMenuAsync(auth0Id, menu);
        
        return Ok(menuDto);
    }

    [Authorize(Roles = "Establishment")]
    [HttpPut("menu/item/image/{menuItemId:int}")]
    public async Task<IActionResult> UpdateMenuItemPictureAsync(int menuItemId, [ContentType([ "image/jpeg", "image/png", "image/gif", "image/webp"])] IFormFile picture)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                      ?? throw new ArgumentException("Auth0Id not found");

        var pictureUrl = await establishmentService.UpdateMenuItemPictureAsync(auth0Id, menuItemId, picture);

        return Ok(new { pictureUrl });
    }

    [Authorize(Roles = "Establishment")]
    [HttpDelete("menu/{menuId:int}")]
    public async Task<IActionResult> DeleteMenuAsync(int menuId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                      ?? throw new ArgumentException("Auth0Id not found");
        
        await establishmentService.DeleteMenuAsync(auth0Id, menuId);
        
        return NoContent();
    }
    
    [Authorize(Roles = "Establishment")]
    [HttpDelete("menu/item/{menuItemId:int}")]
    public async Task<IActionResult> DeleteMenuItemAsync(int menuItemId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                      ?? throw new ArgumentException("Auth0Id not found");
        
        await establishmentService.DeleteMenuItemAsync(auth0Id, menuItemId);
        
        return NoContent();
    }
}