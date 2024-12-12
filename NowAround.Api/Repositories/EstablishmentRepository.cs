using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Responses;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Utilities;

namespace NowAround.Api.Repositories;

public class EstablishmentRepository : BaseAccountRepository<Establishment>, IEstablishmentRepository
{
    public EstablishmentRepository(AppDbContext context, ILogger<Establishment> logger) 
        : base(context, logger)
    {
    }

    public new async Task<EstablishmentProfileResponse> GetByAuth0IdAsync(string auth0Id)
    {
        try
        {
            var establishment = await DbSet
                .Where(e => e.Auth0Id == auth0Id)
                .Select(e => new EstablishmentProfileResponse(
                    e.Auth0Id,
                new GenericInfo(
                    e.Name,
                    null,
                    e.EstablishmentTags.Select(et => et.Tag.Name).ToList(),
                    e.EstablishmentCategories.Select(ec => ec.Category.Name).ToList(),
                    e.PriceCategory.ToString(),
                    e.EstablishmentCuisines.Select(ec => ec.Cuisine.Name).ToList(),
                    e.SocialLinks.Select(sl => new SocialLinkDto(sl.Name, sl.Url)).ToList()
                ),
                new LocationInfo(
                    e.Address,
                    e.City,
                    new BusinessHoursDto(
                        e.BusinessHours.Monday,
                        e.BusinessHours.Tuesday,
                        e.BusinessHours.Wednesday,
                        e.BusinessHours.Thursday,
                        e.BusinessHours.Friday,
                        e.BusinessHours.Saturday,
                        e.BusinessHours.Sunday,
                        e.BusinessHours.BusinessHoursExceptions
                            .Select(bhe => new BusinessHoursExceptionsDto(bhe.Date, bhe.Status))
                            .ToList()
                    ),
                    e.Longitude,
                    e.Latitude
                ),
                new List<PostWithAuthIdsResponse>(
                    e.Posts.Select(p => new PostWithAuthIdsResponse(
                        null,
                        p.Headline,
                        p.Body,
                        p.ImageUrl,
                        p.PostLikes.Select(pl => pl.User.Auth0Id).ToList(),
                        p.CreatedAt
                    )).ToList()
                    ),
                e.Menus.Select(m => new MenuDto(
                    m.Name,
                    m.MenuItems.Select(mi => new MenuItemDto(
                        mi.Name,
                        mi.PhotoUrl,
                        mi.Description,
                        mi.Price.ToString()
                    )).ToList()
                )).ToList(),
                new RatingStatisticResponse(
                    e.RatingStatistic.OneStar,
                    e.RatingStatistic.TwoStars,
                    e.RatingStatistic.ThreeStars,
                    e.RatingStatistic.FourStars,
                    e.RatingStatistic.FiveStars,
                    e.RatingStatistic.Reviews.Select(r => new ReviewWithAuthIdsResponse(
                        r.User.Auth0Id,
                        null,
                        r.User.FullName,
                        r.Body,
                        r.Rating,
                        r.CreatedAt
                    )).ToList()
                    
                )
            ))
            .FirstOrDefaultAsync();
            if (establishment == null)
            {
                Logger.LogWarning("Establishment with Auth0 ID {Auth0Id} not found", auth0Id);
                throw new EntityNotFoundException("Establishment", "Auth0 ID", auth0Id);
            }
            return establishment;
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to get establishment by Auth0 ID: {Message}", e.Message);
            throw new Exception("Failed to get establishment by Auth0 ID", e);
        }
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
    
    public async Task<List<EstablishmentDto>> GetRangeWithFilterAsync(SearchValues searchValues, int page)
    {
        try
        {
            var query = DbSet.AsQueryable();
            
            query = EstablishmentSearchQueryBuilder.ApplyFilters(query, searchValues);

            var establishments = new List<EstablishmentDto>();
            
            if (page > 0)
            {
                establishments = await query
                    .Skip((page - 1) * 5).Take(5)
                    .Select(e => new EstablishmentDto
                    {
                        Auth0Id = e.Auth0Id,
                        Name = e.Name
                    })
                    .ToListAsync();
            }
            else
            {
                establishments = await query
                    .Select(e => new EstablishmentDto
                    {
                        Auth0Id = e.Auth0Id,
                        Name = e.Name,
                        Latitude = e.Latitude,
                        Longitude = e.Longitude
                    })
                    .ToListAsync();
            }
            
            if (establishments.Count == 0)
            {
                Logger.LogInformation("No establishments were found with the given filter values");
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