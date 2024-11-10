using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Enum;
using NowAround.Api.Utilities;

namespace NowAround.Api.Repositories;

public class EstablishmentRepository : IEstablishmentRepository
{
    
    private readonly AppDbContext _context;
    private readonly ILogger<EstablishmentRepository> _logger;
    
    public EstablishmentRepository(AppDbContext context, ILogger<EstablishmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Checks if an establishment with the given name exists.
    /// </summary>
    /// <param name="name"> The name of the establishment </param>
    /// <returns> True if establishment exists, false if not </returns>
    /// <exception cref="Exception"> Failed to check if establishment exists by name </exception>
    public async Task<bool> CheckIfEstablishmentExistsByNameAsync(string name)
    {
        try
        {
            return await _context.Establishments.AnyAsync(e => e.Name == name);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to check if establishment exists by name: {Message}", e.Message);
            throw new Exception("Failed to check if establishment exists by name", e);
        }
    }
    
    /// <summary>
    /// Creates a new establishment.
    /// </summary>
    /// <param name="establishment"> The establishment to create </param>
    /// <returns> ID of the created establishment </returns>
    /// <exception cref="Exception"> Failed to create establishment </exception>
    public async Task<int> CreateEstablishmentAsync(Establishment establishment)
    {
        try
        {
            await _context.Establishments.AddAsync(establishment);
            await _context.SaveChangesAsync();
            return establishment.Id;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to create establishment: {Message}", e.Message);
            throw new Exception("Failed to create establishment", e);
        }
    }
    
    /// <summary>
    /// Gets an establishment by ID.
    /// </summary>
    /// <param name="id"> The ID of the establishment </param>
    /// <returns> Establishment or null </returns>
    /// <exception cref="Exception"> Failed to get establishment by ID </exception>
    public async Task<Establishment?> GetEstablishmentByIdAsync(int id)
    {
        try
        {
            var establishment = await _context.Establishments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id);
            if (establishment == null)
            {
                _logger.LogWarning("Establishment with ID {id} not found", id);
                return null;
            }
            
            return establishment;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get establishment by ID: {Message}", e.Message);
            throw new Exception("Failed to get establishment by ID", e);
        }
    }
    
    /// <summary>
    /// Gets an establishment by Auth0 ID.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment </param>
    /// <returns> Establishment or null </returns>
    /// <exception cref="Exception"> Failed to get establishment by Auth0ID </exception>
    public async Task<Establishment?> GetEstablishmentByAuth0IdAsync(string auth0Id)
    {
        try
        {
            var establishment = await _context.Establishments
                .IgnoreQueryFilters()
                .Include(e => e.EstablishmentCategories).ThenInclude(ec => ec.Category)
                .Include(e => e.EstablishmentTags).ThenInclude(ec => ec.Tag)
                .FirstOrDefaultAsync(e => e.Auth0Id == auth0Id);
            if (establishment == null)
            {
                _logger.LogWarning("Establishment with Auth0ID {auth0Id} not found", auth0Id);
                return null;
            }
            
            return establishment;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get establishment by Auth0ID: {Message}", e.Message);
            throw new Exception("Failed to get establishment by Auth0ID", e);
        }
    }
    
    /// <summary>
    /// Gets establishments with pending register status.
    /// </summary>
    /// <returns> List of establishments or null </returns>
    /// <exception cref="Exception"> Failed to get pending establishments </exception>
    public async Task<List<Establishment>?> GetEstablishmentsWithPendingRegisterStatusAsync()
    {
        try
        {
            var establishments = await _context.Establishments
                .IgnoreQueryFilters()
                .Where(e => e.RequestStatus == RequestStatus.Pending)
                .ToListAsync();
            if (establishments.Count == 0)
            {
                _logger.LogWarning("Pending establishments not found");
                return null;
            }
            
            return establishments;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get pending establishments: {Message}", e.Message);
            throw new Exception("Failed to get pending establishments", e);
        }
    }
    
    /// <summary>
    /// Gets establishments with filter.
    /// Query is being added if the filter value is not null or empty.
    /// </summary>
    /// <param name="name"> Establishment name </param>
    /// <param name="categoryName"> Category name </param>
    /// <param name="tagNames"> List of tag names </param>
    /// <returns> List of establishments or null </returns>
    /// <exception cref="InvalidOperationException"> Failed to get establishments by filter </exception>
    public async Task<List<Establishment>?> GetEstablishmentsWithFilterAsync(string? name, string? categoryName, List<string>? tagNames)
    {
        try
        {
            var query = _context.Establishments.AsQueryable();
            
            query = EstablishmentFilterQueryBuilder.ApplyFilters(query, name, categoryName, tagNames);
            
            var establishments = await query.ToListAsync();
            
            if (establishments.Count == 0)
            {
                _logger.LogWarning("Establishments with filter not found");
                return null;
            }
            
            return establishments;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get establishments by filter: {Message}", e.Message);
            throw new InvalidOperationException("Failed to get establishments by filter", e);
        }
    }
    
    /// <summary>
    /// Gets establishments with filter in area.
    /// Query is being added if the filter value is not null or empty.
    /// </summary>
    /// <param name="nwLat"></param>
    /// <param name="nwLong"></param>
    /// <param name="seLat"></param>
    /// <param name="seLong"></param>
    /// <param name="name"> Establishment name </param>
    /// <param name="categoryName"> Category name </param>
    /// <param name="tagNames"> List of tag names </param>
    /// <returns> List of establishments or null </returns>
    /// <exception cref="Exception"> Failed to get establishments by area </exception>
    public async Task<List<Establishment>?> GetEstablishmentsWithFilterInAreaAsync(double nwLat, double nwLong, double seLat, double seLong, string? name, string? categoryName, List<string>? tagNames)
    {
        try
        {
            var query = _context.Establishments
                .Where(e => e.Latitude <= nwLat && e.Latitude >= seLat)
                .Where(e => e.Longitude >= nwLong && e.Longitude <= seLong);
            
            query = EstablishmentFilterQueryBuilder.ApplyFilters(query, name, categoryName, tagNames);
            
            var establishments = await query.ToListAsync();
            
            if (establishments.Count == 0)
            {
                _logger.LogWarning("Establishments in area not found");
                return null;
            }
            
            return establishments;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get establishments by area: {Message}", e.Message);
            throw new Exception("Failed to get establishments by area", e);
        }
    }
    
    /// <summary>
    /// Gets the count of establishments created between the specified start and end dates.
    /// </summary>
    /// <param name="startDate"> The start date of the range </param>
    /// <param name="endDate"> The end date of the range </param>
    /// <returns> The count of establishments created between the specified dates </returns>
    /// <exception cref="Exception"> Thrown when there is an error retrieving the count of establishments </exception>
    public async Task<int> GetEstablishmentsCountByCreatedAtBetweenDatesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            return await _context.Establishments
                .CountAsync(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get establishments count by created at between dates: {Message}", e.Message);
            throw new Exception("Failed to get establishments count by created at between dates", e);
        }
    }

    /// <summary>
    /// Updates an establishment.
    /// Categories and tags are being updated separately.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment to update </param>
    /// <param name="establishmentDto"> DTO with updated establishment properties </param>
    /// <returns> True if establishment was updated, false if not found </returns>
    /// <exception cref="InvalidOperationException"> Failed to update establishment </exception>
    public async Task<bool> UpdateEstablishmentByAuth0IdAsync(string auth0Id, EstablishmentDto establishmentDto)
    {
        try
        {
            var establishment = await _context.Establishments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Auth0Id == auth0Id);
            
            if (establishment == null)
            {
                _logger.LogWarning("Establishment with Auth0 ID {Auth0Id} not found", auth0Id);
                return false;
            }

            // For each property in the DTO, update the corresponding property in the entity if it is provided
            foreach (var property in typeof(EstablishmentDto).GetProperties())
            {
                var newValue = property.GetValue(establishmentDto);
                if (newValue != null)
                {
                    var establishmentProperty = typeof(Establishment).GetProperty(property.Name);
                    if (property.Name != "EstablishmentCategories" && property.Name != "EstablishmentTags")
                    {
                        establishmentProperty?.SetValue(establishment, newValue);
                    }
                }
            }

            // If categories are provided, update them
            if (establishmentDto.EstablishmentCategories != null && establishmentDto.EstablishmentCategories.Count != 0)
            {
                // Remove all existing categories and add new ones
                await _context.EstablishmentCategories
                    .Where(ec => ec.EstablishmentId == establishment.Id)
                    .ForEachAsync(ec => _context.EstablishmentCategories.Remove(ec));
                establishment.EstablishmentCategories = establishmentDto.EstablishmentCategories;
            }

            // If tags are provided, update them
            if (establishmentDto.EstablishmentTags != null && establishmentDto.EstablishmentTags.Count != 0)
            {
                // Remove all existing tags and add new ones
                await _context.EstablishmentTags
                    .Where(et => et.EstablishmentId == establishment.Id)
                    .ForEachAsync(et => _context.EstablishmentTags.Remove(et));
                establishment.EstablishmentTags = establishmentDto.EstablishmentTags;
            }
            
            establishment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to update establishment register request: {Message}", e.Message);
            throw new InvalidOperationException("Failed to update establishment register request", e);
        }
    }
    
    /// <summary>
    /// Deletes an establishment by Auth0 ID.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment to delete </param>
    /// <returns> True if establishment was deleted, false if not found </returns>
    /// <exception cref="InvalidOperationException"> Failed to delete establishment </exception>
    public async Task<bool> DeleteEstablishmentByAuth0IdAsync(string auth0Id)
    {
        try
        {
            var establishment = await _context.Establishments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Auth0Id == auth0Id);
            if (establishment == null)
            {
                _logger.LogWarning("Establishment with Auth0 ID {Auth0Id} not found", auth0Id);
                return false;
            }
            
            _context.Establishments.Remove(establishment);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to delete establishment: {Message}", e.Message);
            throw new InvalidOperationException("Failed to delete establishment", e);
        }
    }
}