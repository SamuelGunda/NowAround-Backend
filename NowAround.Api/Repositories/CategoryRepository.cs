using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories;

public class CategoryRepository : ICategoryRepository
{
    
    private readonly AppDbContext _context;
    private readonly ILogger<CategoryRepository> _logger;
    
    public CategoryRepository(AppDbContext context, ILogger<CategoryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Retrieves a category by its name.
    /// </summary>
    /// <param name="name"> The name of the category to retrieve </param>
    /// <returns> The task result contains the category if found otherwise null </returns>
    /// <exception cref="Exception"> Thrown when the operation fails </exception>
    public async Task<Category?> GetCategoryByNameAsync(string name)
    {
        try
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == name);
            if (category == null)
            {
                _logger.LogWarning("Category with name {name} does not exist", name);
                return null;
            }
            
            return category;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get category by name: {Message}", e.Message);
            throw new Exception("Failed to get category by name", e);
        }
    }
    
    /// <summary>
    /// Retrieves a category by its name, including its associated tags.
    /// </summary>
    /// <param name="name"> The name of the category to retrieve </param>
    /// <returns> The task result contains the category with its tags if found otherwise, null </returns>
    /// <exception cref="Exception"> Thrown when the operation fails </exception>
    public async Task<Category?> GetCategoryByNameWithTagsAsync(string name)
    {
        try
        {
            var category = await _context.Categories
                .Include(c => c.Tags)
                .FirstOrDefaultAsync(c => c.Name == name);
            if (category == null)
            {
                _logger.LogWarning("Category with name {name} does not exist", name);
                return null;
            }
            
            return category;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get category by name: {Message}", e.Message);
            throw new Exception("Failed to get category by name", e);
        }
    }
}