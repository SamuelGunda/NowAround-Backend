using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<int> CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);
}

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IBaseEntity
{
    protected AppDbContext Context { get; }
    protected DbSet<TEntity> DbSet { get; }
    protected ILogger<TEntity> Logger { get; }

    protected BaseRepository(AppDbContext context, ILogger<TEntity> logger)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();
        Logger = logger;
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<int> CreateAsync(TEntity entity)
    {
        try
        {
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();

            if (entity.Id == 0)
            {
                Logger.LogError("Failed to create {EntityType}. Id is 0.", typeof(TEntity).Name);
                throw new Exception($"Failed to create {typeof(TEntity).Name}. Id is 0.");
            }
            
            return entity.Id;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to create entity of type {EntityType}", typeof(TEntity).Name);
            throw new Exception($"Failed to create entity of type {typeof(TEntity).Name}", e);
        }
    }

    public async Task UpdateAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}