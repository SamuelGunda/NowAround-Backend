using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories.Interfaces;

namespace NowAround.Api.Repositories;

public class EventRepository : BaseRepository<Event>, IEventRepository
{
    public EventRepository(AppDbContext context, ILogger<Event> logger) 
        : base(context, logger)
    {
    }
}