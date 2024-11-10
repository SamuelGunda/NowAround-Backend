using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories;

public class TagRepository : ITagRepository
{
    
    private readonly AppDbContext _context;
    private readonly ILogger<TagRepository> _logger;
    
    public TagRepository(AppDbContext context, ILogger<TagRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Retrieves tag by its name.
    /// </summary>
    /// <param name="name"> The name of the tag to retrieve </param>
    /// <returns> The task result contains the tag if found otherwise null </returns>
    /// <exception cref="Exception"> Thrown when the operation fails </exception>
    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        try
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name == name);
            if (tag == null)
            {
                _logger.LogWarning("Tag with name {name} does not exist", name);
                return null;
            }
            
            return tag;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get tag by name: {Message}", e.Message);
            throw new Exception("Failed to get tag by name", e);
        }
    }
}