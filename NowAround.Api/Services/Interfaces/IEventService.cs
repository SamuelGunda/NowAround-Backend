using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Requests;

namespace NowAround.Api.Services.Interfaces;

public interface IEventService
{
    Task<EventDto> CreateEventAsync(string auth0Id, EventCreateRequest eventCreateRequest);
    Task ReactToEventAsync(int eventId, string auth0Id);
    Task DeleteEventAsync(string auth0Id, int eventId);
}