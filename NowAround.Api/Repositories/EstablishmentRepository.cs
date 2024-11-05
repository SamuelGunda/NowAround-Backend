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
            throw new Exception("Failed to get establishments by filter", e);
        }
    }
    
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

        if (establishmentDto.EstablishmentCategories != null && establishmentDto.EstablishmentCategories.Count != 0)
        {
            // Remove all existing categories and add new ones
            await _context.EstablishmentCategories
                .Where(ec => ec.EstablishmentId == establishment.Id)
                .ForEachAsync(ec => _context.EstablishmentCategories.Remove(ec));
            establishment.EstablishmentCategories = establishmentDto.EstablishmentCategories;
        }

        if (establishmentDto.EstablishmentTags != null && establishmentDto.EstablishmentTags.Count != 0)
        {
            // Remove all existing tags and add new ones
            await _context.EstablishmentTags
                .Where(et => et.EstablishmentId == establishment.Id)
                .ForEachAsync(et => _context.EstablishmentTags.Remove(et));
            establishment.EstablishmentTags = establishmentDto.EstablishmentTags;
        }

        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception e)
    {
        _logger.LogError("Failed to update establishment register request: {Message}", e.Message);
        throw new InvalidOperationException("Failed to update establishment register request", e);
    }
}
    
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