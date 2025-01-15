using NowAround.Application.Dtos;
using NowAround.Application.Requests;

namespace NowAround.Application.Services;

public interface IEventService
{
    Task<EventDto> CreateEventAsync(string auth0Id, EventCreateRequest eventCreateRequest);
    Task ReactToEventAsync(int eventId, string auth0Id);
    Task DeleteEventAsync(string auth0Id, int eventId);
}