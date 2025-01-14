using Microsoft.Extensions.Logging;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;
using NowAround.Infrastructure.Context;
using NowAround.Infrastructure.Repository.Base;

namespace NowAround.Infrastructure.Repository.Specific;

public class PostRepository : BaseRepository<Post>, IPostRepository
{
    public PostRepository(AppDbContext context, ILogger<Post> logger) 
        : base(context, logger)
    {
    }
}