using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;
    
    public TagRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Tag> GetTagByNameAsync(string name)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Name == name);
        
        return tag ?? throw new InvalidOperationException($"Tag with name {name} does not exist");
    }
}