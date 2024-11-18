using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories.Interfaces;

namespace NowAround.Api.Repositories;

public class TagRepository : BaseRepository<Tag>, ITagRepository
{
    public TagRepository(AppDbContext context, ILogger<Tag> logger) 
        : base(context, logger)
    {
    }
}