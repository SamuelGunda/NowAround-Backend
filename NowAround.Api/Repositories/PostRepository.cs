using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories.Interfaces;

namespace NowAround.Api.Repositories;

public class PostRepository : BaseRepository<Post>, IPostRepository
{
    public PostRepository(AppDbContext context, ILogger<Post> logger) 
        : base(context, logger)
    {
    }
}