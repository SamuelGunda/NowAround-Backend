using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories.Interfaces;

namespace NowAround.Api.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    
    public CategoryRepository(AppDbContext context, ILogger<Category> logger) 
        : base(context, logger)
    {
    }

    public async Task<Category?> GetByNameWithTagsAsync(string name)
    {
        try
        {
            var category = await Context.Categories
                .Include(c => c.Tags)
                .FirstOrDefaultAsync(c => c.Name == name);
            if (category == null)
            {
                Logger.LogWarning("Category with name {name} does not exist", name);
                return null;
            }
            
            return category;
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to get category by name: {Message}", e.Message);
            throw new Exception("Failed to get category by name", e);
        }
    }
}