using Microsoft.Extensions.Logging;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;
using NowAround.Infrastructure.Context;
using NowAround.Infrastructure.Repository.Base;

namespace NowAround.Infrastructure.Repository.Specific;

public class EventRepository : BaseRepository<Event>, IEventRepository
{
    public EventRepository(AppDbContext context, ILogger<Event> logger) 
        : base(context, logger)
    {
    }
}