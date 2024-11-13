using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<int> CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);
}

public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected AppDbContext Context { get; }
    protected DbSet<T> DbSet { get; }
    protected ILogger<T> Logger { get; }

    protected BaseRepository(AppDbContext context, ILogger<T> logger)
    {
        Context = context;
        DbSet = Context.Set<T>();
        Logger = logger;
    }

    public async Task<T> GetByIdAsync(int id)
    {
        try
        {
            var entity = await DbSet.FindAsync(id);
            if (entity == null)
            {
                Logger.LogError("Failed to get {EntityType} by ID. Entity not found.", typeof(T).Name);
                throw new Exception($"Failed to get {typeof(T).Name} by ID. Entity not found.");
            }

            return entity;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to get {EntityType} by ID", typeof(T).Name);
            throw new Exception($"Failed to get {typeof(T).Name} by ID", e);
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            return await DbSet.ToListAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to get all {EntityType}", typeof(T).Name);
            throw new Exception($"Failed to get all {typeof(T).Name}", e);
        }
    }

    public async Task<int> CreateAsync(T entity)
    {
        try
        {
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();

            if (entity.Id == 0)
            {
                Logger.LogError("Failed to create {EntityType}. Id is 0.", typeof(T).Name);
                throw new Exception($"Failed to create {typeof(T).Name}. Id is 0.");
            }
            
            return entity.Id;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to create entity of type {EntityType}", typeof(T).Name);
            throw new Exception($"Failed to create entity of type {typeof(T).Name}", e);
        }
    }

    public async Task UpdateAsync(T entity)
    {
        try
        {
            DbSet.Update(entity);
            await Context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to update entity of type {EntityType}", typeof(T).Name);
            throw new Exception($"Failed to update entity of type {typeof(T).Name}", e);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await DbSet.FindAsync(id);
            if (entity == null)
            {
                Logger.LogError("Failed to delete {EntityType} by ID. Entity not found.", typeof(T).Name);
                throw new Exception($"Failed to delete {typeof(T).Name} by ID. Entity not found.");
            }

            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to delete {EntityType} by ID", typeof(T).Name);
            throw new Exception($"Failed to delete {typeof(T).Name} by ID", e);
        }
    }
}