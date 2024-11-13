using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Repositories;

public interface IBaseAccountRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseAccountEntity
{
    public Task<TEntity?> GetByAuth0IdAsync(string auth0Id);
    public Task<int> GetCountByCreatedAtBetweenDatesAsync(DateTime startDate, DateTime endDate);
    public Task<bool> DeleteByAuth0IdAsync(string auth0Id);
}

public abstract class BaseAccountRepository<TEntity> : BaseRepository<TEntity>, IBaseAccountRepository<TEntity> where TEntity : BaseAccountEntity
{
    public BaseAccountRepository(AppDbContext context, ILogger<TEntity> logger) : base(context, logger)
    {
    }

    public async Task<TEntity?> GetByAuth0IdAsync(string auth0Id)
    {
        try
        {
            var entity = await DbSet
                .FirstOrDefaultAsync(e => e.Auth0Id == auth0Id);
            if (entity == null)
            {
                Logger.LogError("Failed to get entity by Auth0 ID. Entity not found.");
                return null;
            }
            
            return entity;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to get {EntityType}  by Auth0 ID", typeof(TEntity).Name);
            throw new Exception($"Failed to get {typeof(TEntity).Name} by Auth0 ID", e);
        }
    }
    
    public async Task<int> GetCountByCreatedAtBetweenDatesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            return await DbSet
                .CountAsync(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to get {EntityType} count", typeof(TEntity).Name);
            throw new Exception($"Failed to get {typeof(TEntity).Name} count", e);
        }
    }
    public async Task<bool> DeleteByAuth0IdAsync(string auth0Id)
    {
        try
        {
            var establishment = await DbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Auth0Id == auth0Id);
            if (establishment == null)
            {
                Logger.LogWarning("{EntityType} with Auth0 ID {auth0Id} does not exist", typeof(TEntity).Name, auth0Id);
                return false;
            }
            
            DbSet.Remove(establishment);
            await Context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to delete {EntityType} by Auth0 ID", typeof(TEntity).Name);
            throw new Exception($"Failed to delete {typeof(TEntity).Name} by Auth0 ID", e);
        }
    }
    
}