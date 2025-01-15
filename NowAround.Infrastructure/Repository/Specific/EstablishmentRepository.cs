using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NowAround.Application.Common.Exceptions;
using NowAround.Application.Dtos;
using NowAround.Application.Responses;
using NowAround.Domain.Enum;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;
using NowAround.Infrastructure.Context;
using NowAround.Infrastructure.Repository.Base;

namespace NowAround.Infrastructure.Repository.Specific;

public class EstablishmentRepository : BaseAccountRepository<Establishment>, IEstablishmentRepository
{
    public EstablishmentRepository(AppDbContext context, ILogger<Establishment> logger) 
        : base(context, logger)
    {
    }

    public async Task<Establishment> GetProfileByAuth0IdAsync(string auth0Id)
    {
        try
        {
            var establishmentEntity = await DbSet
                .Include(e => e.Tags)
                .Include(e => e.Categories)
                .Include(e => e.SocialLinks)
                .Include(e => e.BusinessHours.BusinessHoursExceptions)
                .Include(e => e.Posts).ThenInclude(p => p.Likes)
                .Include(e => e.Menus).ThenInclude(m => m.MenuItems)
                .Include(e => e.Events).ThenInclude(ev => ev.InterestedUsers)
                .Include(e => e.RatingStatistic.Reviews).ThenInclude(r => r.User)
                .Where(e => e.Auth0Id == auth0Id)
                .FirstOrDefaultAsync();

            if (establishmentEntity == null)
            {
                Logger.LogWarning("Establishment with Auth0 ID {Auth0Id} not found", auth0Id);
                throw new EntityNotFoundException("Establishment", "Auth0 ID", auth0Id);
            }

            return establishmentEntity;
        }
        catch (EntityNotFoundException)
        {
            throw;
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
    
    public async Task<List<Establishment>> GetRangeWithFilterAsync(
        Func<IQueryable<Establishment>, IQueryable<Establishment>> queryBuilder, 
        int page)
    {
        try
        {
            var query = DbSet.AsQueryable();

            query = queryBuilder(query);

            List<Establishment> establishments;

            if (page > 0)
            {
                establishments = await query
                    .Skip((page - 1) * 5).Take(5)
                    .ToListAsync();
            }
            else
            {
                establishments = await query.ToListAsync();
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
}