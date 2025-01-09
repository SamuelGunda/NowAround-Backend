using NowAround.Api.Models.Requests;

namespace NowAround.Api.Services.Interfaces;

public interface IEventService
{
    Task CreateEventAsync(string auth0Id, EventCreateRequest eventCreateRequest);
    
}