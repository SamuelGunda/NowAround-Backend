using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Services.Interfaces;

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
    [HttpPut]
    public async Task<IActionResult> UpdateEstablishmentAsync(EstablishmentUpdateRequest establishmentUpdateRequest)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        await establishmentService.UpdateEstablishmentAsync(auth0Id, establishmentUpdateRequest);
        
        return NoContent();
    }
    
    [Authorize(Roles = "Establishment")]
    [HttpPut ("image/{pictureContext}")]
    public async Task<IActionResult> UpdateEstablishmentPictureAsync([FromRoute] string pictureContext, IFormFile picture)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        await establishmentService.UpdateEstablishmentPictureAsync(auth0Id, pictureContext, picture);
        
        //TODO: Change to NoContent()
        return Created("", new { message = "Picture updated successfully" });
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
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        await establishmentService.DeleteEstablishmentAsync(auth0Id);

        return NoContent();
    }
    
    //Menu Endpoints
    
    [Authorize(Roles = "Establishment")]
    [HttpPost("menu")]
    public async Task<IActionResult> AddMenuAsync(MenuCreateRequest menu)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        await establishmentService.AddMenuAsync(auth0Id, menu);
        
        return Created("", new { message = "Menu added successfully" });
    }

    [Authorize(Roles = "Establishment")]
    [HttpPut("menu/{menuId:int}")]
    public async Task<IActionResult> UpdateMenuAsync(int menuId, MenuCreateRequest menu)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        await establishmentService.UpdateMenuAsync(auth0Id, menuId, menu);
        
        return Created("", new { message = "Menu updated successfully" });
    }

    [Authorize(Roles = "Establishment")]
    [HttpPut("menu/item/image/{menuItemId:int}")]
    public async Task<IActionResult> UpdateMenuItemPictureAsync(int menuItemId, IFormFile image)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ??
                      throw new ArgumentException("Auth0Id not found");

        await establishmentService.UpdateMenuItemPictureAsync(auth0Id, menuItemId, image);

        return Created("", new { message = "Menu item picture updated successfully" });
    }

    [Authorize(Roles = "Establishment")]
    [HttpDelete("menu/{menuId:int}")]
    public async Task<IActionResult> DeleteMenuAsync(int menuId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        await establishmentService.DeleteMenuAsync(auth0Id, menuId);
        
        return NoContent();
    }
    
    [Authorize(Roles = "Establishment")]
    [HttpDelete("menu/item/{menuItemId:int}")]
    public async Task<IActionResult> DeleteMenuItemAsync(int menuItemId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("Auth0Id not found");
        
        await establishmentService.DeleteMenuItemAsync(auth0Id, menuItemId);
        
        return NoContent();
    }
}