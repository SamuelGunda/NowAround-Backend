using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NowAround.Application.Common.Exceptions;
using NowAround.Domain.Common;
using NowAround.Domain.Interfaces.Base;
using NowAround.Infrastructure.Context;

namespace NowAround.Infrastructure.Repository.Base;

//TODO: Change Base Repository from abstract to normal class
public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected AppDbContext Context { get; }
    protected DbSet<T> DbSet { get; }
    protected ILogger<T> Logger { get; }

    public BaseRepository(AppDbContext context, ILogger<T> logger)
    {
        Context = context;
        DbSet = Context.Set<T>();
        Logger = logger;
    }

    public async Task<int> CreateAsync(T entity)
    {
        try
        {
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
            
            return entity.Id;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to create {EntityType}: {message}", typeof(T).Name, e.Message);
            throw new Exception($"Failed to create {typeof(T).Name}", e);
        }
    }
    
    public async Task<bool> CheckIfExistsByPropertyAsync(string propertyName, object propertyValue)
    {
        try
        {
            return await DbSet.AnyAsync(e => EF.Property<object>(e, propertyName) == propertyValue);
        }
        catch (InvalidOperationException e)
        {
            Logger.LogWarning("{EntityType} does not have property {propertyName}: {message}", typeof(T).Name, propertyName, e.Message);
            throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to check if {EntityType} exists by {propertyName}: {message}", typeof(T).Name, propertyName, ex.Message);
            throw new Exception($"Failed to check if {typeof(T).Name} exists by {propertyName}", ex);
        }
    }
    
    public async Task<T?> GetByIdAsync(int id)
    {
        try
        {
            var entity = await DbSet.FindAsync(id);
            if (entity == null)
            {
                Logger.LogWarning("{EntityType} with ID: {Id} does not exist", typeof(T).Name, id);
                return null;
            }

            return entity;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to get {EntityType} by ID: {message}", typeof(T).Name, e.Message);
            throw new Exception($"Failed to get {typeof(T).Name} by ID", e);
        }
    }

    public Task<T?> GetByPropertyAsync(string propertyName, object propertyValue)
    {
        try
        {
            var entity = DbSet.FirstOrDefault(e => EF.Property<object>(e, propertyName) == propertyValue);
            if (entity == null)
            {
                Logger.LogWarning("{EntityType} with {propertyName}: {propertyValue} does not exist", typeof(T).Name,propertyName ,propertyValue);
                return Task.FromResult<T?>(null);
            }
            return Task.FromResult<T?>(entity);
        }
        catch (InvalidOperationException e)
        {
            Logger.LogWarning("{EntityType} does not have property {propertyName}: {message}", typeof(T).Name, propertyName, e.Message);
            throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get {EntityType} by {propertyName}: {message}", typeof(T).Name, propertyName, ex.Message);
            throw new Exception($"Failed to get {typeof(T).Name} by {propertyName}", ex);
        }
    }
    
    public async Task<T> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        bool tracked = true,
        params Func<IQueryable<T>, IQueryable<T>>[] includeProperties)
    {
        try
        {
            IQueryable<T> query = DbSet;

            foreach (var include in includeProperties)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            var entity = await query.FirstOrDefaultAsync();

            if (entity == null)
            {
                Logger.LogWarning("{EntityType} does not exist", typeof(T).Name);
                throw new EntityNotFoundException(typeof(T).Name, "filter", filter?.ToString() ?? "null");
            }

            return entity;
        }
        catch (EntityNotFoundException e)
        {
            throw;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to get {EntityType}: {message}", typeof(T).Name, e.Message);
            throw new Exception($"Failed to get {typeof(T).Name}", e);
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
            Logger.LogError(e, "Failed to get all {EntityType}s: {message}", typeof(T).Name, e.Message);
            throw new Exception($"Failed to get all {typeof(T).Name}s", e);
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
            Logger.LogError(e, "Failed to update {EntityType}: : {Message}", typeof(T).Name, e.Message);
            throw new Exception($"Failed to update {typeof(T).Name}", e);
        }
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var entity = await DbSet.FindAsync(id);
            if (entity == null)
            {
                Logger.LogError("{EntityType} with ID: {Id} does not exist", typeof(T).Name, id);
                return false;
            }

            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to delete {EntityType} by ID: {Message}", typeof(T).Name, e.Message);
            throw new Exception($"Failed to delete {typeof(T).Name} by ID", e);
        }
    }
    
    public async Task DeleteRangeAsync<TEntity>(IQueryable<TEntity> entities) where TEntity : class
    {
        try
        {
            await entities.ForEachAsync(entity => Context.Set<TEntity>().Remove(entity));
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to delete {EntityType}s: {Message}", typeof(TEntity).Name, e.Message);
            throw new Exception($"Failed to delete {typeof(TEntity).Name}s", e);
        }
    }
}