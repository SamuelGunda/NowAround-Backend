﻿using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Repositories;

public abstract class BaseAccountRepository<T> : BaseRepository<T>, IBaseAccountRepository<T> where T : BaseAccountEntity
{
    protected BaseAccountRepository(AppDbContext context, ILogger<T> logger) : base(context, logger)
    {
    }

    public async Task<T?> GetByAuth0IdAsync(string auth0Id)
    {
        try
        {
            var entity = await DbSet.FirstOrDefaultAsync(e => e.Auth0Id == auth0Id);
            if (entity == null)
            {
                Logger.LogWarning("{EntityType} with Auth0 ID: {auth0Id} does not exist", typeof(T).Name, auth0Id);
                return null;
            }
            
            return entity;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to get {EntityType} by Auth0 ID: {message}", typeof(T).Name, e.Message);
            throw new Exception($"Failed to get {typeof(T).Name} by Auth0 ID", e);
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
            Logger.LogError(e, "Failed to get {EntityType} count: {message}", typeof(T).Name, e.Message);
            throw new Exception($"Failed to get {typeof(T).Name} count", e);
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
                Logger.LogWarning("{EntityType} with Auth0 ID: {auth0Id} does not exist", typeof(T).Name, auth0Id);
                return false;
            }
            
            DbSet.Remove(establishment);
            await Context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to delete {EntityType} by Auth0 ID: {message}", typeof(T).Name, e.Message);
            throw new Exception($"Failed to delete {typeof(T).Name} by Auth0 ID", e);
        }
    }
    
}