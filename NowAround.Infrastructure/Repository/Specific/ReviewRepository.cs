using Microsoft.Extensions.Logging;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;
using NowAround.Infrastructure.Context;
using NowAround.Infrastructure.Repository.Base;

namespace NowAround.Infrastructure.Repository.Specific;

public class ReviewRepository : BaseRepository<Review>, IReviewRepository
{
    public ReviewRepository(AppDbContext context, ILogger<Review> logger) 
        : base(context, logger)
    {
    }
}