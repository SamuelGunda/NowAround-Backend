using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Enum;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Utilities;

namespace NowAround.Api.Repositories;

public class EstablishmentRepository : BaseAccountRepository<Establishment>, IEstablishmentRepository
{
    public EstablishmentRepository(AppDbContext context, ILogger<Establishment> logger) 
        : base(context, logger)
    {
    }
    
    public async Task<List<Establishment>> GetAllWhereRegisterStatusPendingAsync()
    {
        try
        {
            var establishments = await DbSet
                .IgnoreQueryFilters()
                .Where(e => e.RequestStatus == RequestStatus.Pending)
                .ToListAsync();
            if (establishments.Count == 0)
            {
                Logger.LogWarning("Pending establishments not found");
                return [];
            }
            
            return establishments;
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to get pending establishments: {Message}", e.Message);
            throw new Exception("Failed to get pending establishments", e);
        }
    }
    
    public async Task<List<Establishment>> GetRangeWithFilterAsync(string? name, int? priceCategory, string? categoryName, List<string>? tagNames)
    {
        try
        {
            var query = DbSet.AsQueryable();
            
            query = EstablishmentFilterQueryBuilder.ApplyFilters(query, name, priceCategory, categoryName, tagNames);
            
            var establishments = await query.ToListAsync();
            
            if (establishments.Count == 0)
            {
                Logger.LogInformation("Establishments with filter not found");
                return [];
            }
            
            return establishments;
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to get establishments by filter: {Message}", e.Message);
            throw new Exception("Failed to get establishments by filter", e);
        }
    }
    
    public async Task<List<Establishment>> GetRangeWithFilterInAreaAsync(
        double nwLat, double nwLong,
        double seLat, double seLong, 
        string? name, int? priceCategory, string? categoryName, List<string>? tagNames)
    {
        try
        {
            var query = DbSet
                .Where(e => e.Latitude <= nwLat && e.Latitude >= seLat)
                .Where(e => e.Longitude >= nwLong && e.Longitude <= seLong);
            
            query = EstablishmentFilterQueryBuilder.ApplyFilters(query, name, priceCategory, categoryName, tagNames);
            
            var establishments = await query.ToListAsync();
            
            if (establishments.Count == 0)
            {
                Logger.LogInformation("Establishments with filter in area not found");
                return [];
            }
            
            return establishments;
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to get establishments by area: {Message}", e.Message);
            throw new Exception("Failed to get establishments by area", e);
        }
    }

    public async Task UpdateAsync(EstablishmentDto establishmentDto)
    {
        var auth0Id = establishmentDto.Auth0Id;
        
        try
        {
            var establishment = await DbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Auth0Id == auth0Id);
            
            if (establishment == null)
            {
                Logger.LogWarning("Establishment with Auth0 ID {Auth0Id} not found", auth0Id);
                throw new EntityNotFoundException("Establishment", "Auth0 ID", auth0Id);
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
            
            if (establishmentDto.EstablishmentCategories?.Count > 0)
            {
                var categories = Context.EstablishmentCategories.Where(ec => ec.EstablishmentId == establishment.Id);
                await DeleteRangeAsync(categories);

                establishment.EstablishmentCategories = establishmentDto.EstablishmentCategories;
            }
            
            if (establishmentDto.EstablishmentTags?.Count > 0)
            {
                var tags = Context.EstablishmentTags.Where(et => et.EstablishmentId == establishment.Id);
                await DeleteRangeAsync(tags);
                
                establishment.EstablishmentTags = establishmentDto.EstablishmentTags;
            }
            
            establishment.UpdatedAt = DateTime.UtcNow;

            await Context.SaveChangesAsync();
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to update establishment register request: {Message}", e.Message);
            throw new Exception("Failed to update establishment register request", e);
        }
    }
}