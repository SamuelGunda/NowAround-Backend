using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Repositories;

public interface IBaseAccountRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IBaseAccountEntity
{
    public Task<TEntity?> GetByAuth0IdAsync(string auth0Id);
}

public abstract class BaseAccountRepository<TEntity> : BaseRepository<TEntity>, IBaseAccountRepository<TEntity> where TEntity : class, IBaseAccountEntity, IBaseEntity
{
    protected AppDbContext Context { get; }
    protected DbSet<TEntity> DbSet { get; }
    protected ILogger<TEntity> Logger { get; }
    
    public BaseAccountRepository(AppDbContext context, ILogger<TEntity> logger) : base(context, logger)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
        Logger = logger;
    }

    public async Task<TEntity?> GetByAuth0IdAsync(string auth0Id)
    {
        try
        {
            var entity = await DbSet.FirstOrDefaultAsync(e => e.Auth0Id == auth0Id);
            if (entity == null)
            {
                Logger.LogError("Failed to get entity by Auth0 ID. Entity not found.");
                return null;
            }
            
            return entity;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to get entity by Auth0 ID");
            throw new Exception("Failed to get entity by Auth0 ID", e);
        }
    }
}