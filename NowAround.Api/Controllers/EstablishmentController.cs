using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Models.Responses;
using NowAround.Api.Services;
using NowAround.Api.Services.Interfaces;
using NowAround.Api.Utilities;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstablishmentController : ControllerBase
{
    
    private readonly ILogger<EstablishmentController> _logger;
    private readonly IEstablishmentService _establishmentService;
    
    public EstablishmentController(IEstablishmentService establishmentService, ILogger<EstablishmentController> logger)
    {
        _logger = logger;
        _establishmentService = establishmentService;
    }
    
    /// <summary>
    /// Creates a new establishment. The establishment must be approved by an admin before it is visible to users.
    /// </summary>
    /// <param name="establishment"> The establishment registration request containing the establishment details</param>
    /// <returns> A status code indicating the result of the operation</returns>
    /// <response code="201"> Establishment created successfully </response>
    /// <response code="400"> Establishment already exists or Email already in use </response>
    /// <response code="500"> An error occurred while creating the establishment </response>
    [HttpPost]
    public async Task<IActionResult> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishment)
    {
        try
        {
            await _establishmentService.RegisterEstablishmentAsync(establishment);
            return Created("", new { message = "Establishment created successfully" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating establishment");
            throw;
        }
    }
    
    /// <summary>
    /// Returns the establishment with the given Auth0 ID if it exists.
    /// RETURNS ONLY APPROVED ESTABLISHMENTS.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment </param>
    /// <returns> An IActionResult containing the establishment details if found, or a 404 status code if not found </returns>
    /// <response code="200">Returns the establishment with the given Auth0 ID </response>
    /// <response code="404">No establishment with the given Auth0 ID was found </response>
    /// <response code="500">An error occurred while retrieving the establishment </response>
    [HttpGet("{auth0Id}")]
    public async Task<IActionResult> GetEstablishmentByAuth0IdAsync(string auth0Id)
    {
        try
        {
            EstablishmentResponse establishment = await _establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id);
            
            return Ok(establishment);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting establishment");
            throw;
        }
    }
    
    /// <summary>
    /// Returns all establishments that have not been approved,
    /// only an admin can view pending establishments
    /// </summary>
    /// <returns> An IActionResult containing list of pending establishments if found, or a 404 status code if not found </returns>
    /// <response code="200"> Returns the pending establishments </response>
    /// <response code="403"> User does not have permission to view pending establishments </response>
    /// <response code="404"> No pending establishments were found </response>
    /// <response code="500"> An error occurred while retrieving the establishments </response>
    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingEstablishmentsAsync()
    {
        try
        {
            var establishments = await _establishmentService.GetPendingEstablishmentsAsync();
            if (establishments.Count == 0)
            {
                return Ok(new { message = "No pending establishments were found" });
            }
            return Ok(establishments);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting pending establishments");
            throw;
        }
    }
    
    /// <summary>
    /// Returns all establishments that match the given filter criteria.
    /// At least one of the filter criteria must be provided.
    /// RETURNS ONLY APPROVED ESTABLISHMENTS.
    /// </summary>
    /// <param name="name"> The name of the establishment to filter by </param>
    /// <param name="priceCategory"> The price category of the establishment to filter by </param>
    /// <param name="categoryName"> The category name of the establishment to filter by </param>
    /// <param name="tagNames"> The list of tag names to filter by </param>
    /// <returns> An IActionResult containing a list of establishment markers if found, or a 404 status code if not found </returns>
    /// <response code="200"> Returns the establishment markers that match the given filter criteria </response>
    /// <response code="404"> No establishments with these criteria were found </response>
    /// <response code="500"> An error occurred while retrieving the establishments </response>
    [HttpGet("search")]
    public async Task<IActionResult> GetEstablishmentMarkersWithFilterAsync(string? name, int? priceCategory, string? categoryName, string? tagNames)
    {
        
        var tagNamesList = tagNames?.Split(',').ToList();
        
        SearchValidator.ValidateFilterValues(name, priceCategory, categoryName, tagNamesList, true);
        
        try
        {
            var establishmentMarker = await _establishmentService.GetEstablishmentMarkersWithFilterAsync(name, priceCategory, categoryName, tagNamesList);
            if (establishmentMarker.Count == 0)
            {
                return Ok(new { message = "No establishments with these criteria were found" });
            }
            return Ok(establishmentMarker);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting establishments in area");
            throw;
        }
    }
    
    /// <summary>
    /// Returns all establishments that match the given filter criteria and are within the given map bounds,
    /// filter criteria are optional.
    /// RETURNS ONLY APPROVED ESTABLISHMENTS
    /// </summary>
    /// <param name="northWestLat"></param>
    /// <param name="northWestLong"></param>
    /// <param name="southEastLat"></param>
    /// <param name="southEastLong"></param>
    /// <param name="name"> The name of the establishment to filter by </param>
    /// <param name="priceCategory"> The price category of the establishment to filter by </param>
    /// <param name="categoryName"> The category name of the establishment to filter by </param>
    /// <param name="tagNames"> The list of tag names to filter by </param>
    /// <returns> An IActionResult containing list of establishment markers if found, or a 404 status code if not found </returns>
    /// <response code="200"> Returns the establishment markers that match the given filter criteria and are within the given map bounds </response>
    /// <response code="404"> No markers found for the specified location </response>
    /// <response code="500"> An error occurred while retrieving the establishments </response>
    [HttpGet("search-area")]
    public async Task<IActionResult> GetEstablishmentMarkersWithFilterInAreaAsync(
        double northWestLat, double northWestLong, 
        double southEastLat, double southEastLong, 
        string? name, int? priceCategory, string? categoryName, string? tagNames)
    {
        
        var tagNamesList = tagNames?.Split(',').ToList();
     
        SearchValidator.ValidateFilterValues(name, priceCategory, categoryName, tagNamesList, false);
        SearchValidator.ValidateMapBounds(northWestLat, northWestLong, southEastLat, southEastLong);
        
        var mapBounds = new MapBounds
        {
            NwLat = northWestLat,
            NwLong = northWestLong,
            SeLat = southEastLat,
            SeLong = southEastLong
        };
        
        try
        {
            var establishmentMarker = await _establishmentService.GetEstablishmentMarkersWithFilterInAreaAsync(mapBounds, name, priceCategory, categoryName, tagNamesList);
            if (establishmentMarker.Count == 0)
            {
                return Ok(new { message = "No markers found for the specified location" });
            }
            return Ok(establishmentMarker);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting establishments in area");
            throw;
        }
    }
    
    /// <summary>
    /// Updates the establishment with the given Auth0 ID,
    /// only the establishment owner or an admin can update the establishment
    /// </summary>
    /// <param name="establishmentUpdateRequest"> The establishment update request containing the updated establishment details </param>
    /// <returns> A status code indicating the result of the operation </returns>
    /// <response code="200"> Establishment updated successfully </response>
    /// <response code="403"> User does not have permission to update this establishment </response>
    /// <response code="404"> No establishment with the given Auth0 ID was found </response>
    /// <response code="500"> An error occurred while updating the establishment </response>
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateEstablishmentAsync(EstablishmentUpdateRequest establishmentUpdateRequest)
    {
        try
        {
            if (AuthorizationHelper.HasAdminOrMatchingEstablishmentId(User, establishmentUpdateRequest.Auth0Id))
            {
                await _establishmentService.UpdateEstablishmentAsync(establishmentUpdateRequest);
                return Ok( new { message = "Establishment updated successfully" });
            }

            return Forbid();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating establishment");
            throw;
        }
    }
    
    /// <summary>
    /// Updates the register status of the establishment with the given Auth0 ID,
    /// only an admin can update the establishment register status
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment </param>
    /// <param name="action"> The action to perform on the establishment register request </param>
    /// <returns> A status code indicating the result of the operation </returns>
    /// <response code="200"> Establishment register status updated successfully </response>
    /// <response code="400"> Invalid action type </response>
    /// <response code="403"> User does not have permission to update this establishment </response>
    /// <response code="404"> No establishment with the given Auth0 ID was found </response>
    /// <response code="500"> An error occurred while updating the establishment register status </response>
    [Authorize(Roles = "Admin")]
    [HttpPut("register-status")]
    public async Task<IActionResult> UpdateEstablishmentRegisterRequestAsync(string auth0Id, string action)
    {
        if (Enum.TryParse<RequestStatus>(action + "ed", true, out var status) && 
            (status == RequestStatus.Accepted || status == RequestStatus.Rejected))
        {
            await _establishmentService.UpdateEstablishmentRegisterRequestAsync(auth0Id, status);
            return Ok(new { message = $"Request successfully {action.ToLower()}ed." });
        }

        _logger.LogWarning("Invalid action type provided: {Action}", action);
        return BadRequest(new { message = "Invalid action type. Please use 'accept' or 'reject'." });
    }
    
    /// <summary>
    /// Deletes the establishment with the given Auth0 ID
    /// Only the establishment owner or an admin can delete the establishment
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment </param>
    /// <returns> A status code indicating the result of the operation </returns>
    /// <response code="200"> Establishment deleted successfully </response>
    /// <response code="403"> User does not have permission to delete this establishment </response>
    /// <response code="404"> No establishment with the given Auth0 ID was found </response>
    /// <response code="500"> An error occurred while deleting the establishment </response>
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteEstablishmentAsync(string auth0Id)
    {
        try
        {
            if (AuthorizationHelper.HasAdminOrMatchingEstablishmentId(User, auth0Id))
            {
                await _establishmentService.DeleteEstablishmentAsync(auth0Id);
                return Ok(new { message = "Establishment deleted successfully" });
            }

            return Forbid();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting establishment");
            throw;
        }
    }
}