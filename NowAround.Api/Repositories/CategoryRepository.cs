using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    
    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Category> GetCategoryByNameAsync(string name)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == name);
        
        return category ?? throw new InvalidOperationException($"Category with name {name} does not exist");
    }
    
    public async Task<Category> GetCategoryByNameWithTagsAsync(string name)
    {
        var category = await _context.Categories
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Name == name);
        
        return category ?? throw new InvalidOperationException($"Category with name {name} does not exist");
    }
}