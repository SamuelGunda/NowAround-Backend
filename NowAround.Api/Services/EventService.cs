using Microsoft.EntityFrameworkCore;
using NowAround.Api.Apis.Mapbox.Services.Interfaces;
using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Services;

public class EventService : IEventService
{
    private readonly IEstablishmentService _establishmentService;
    private readonly IUserService _userService;
    private readonly IMapboxService _mapboxService;
    private readonly IStorageService _storageService;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<Event> _logger;

    public EventService(
        IEstablishmentService establishmentService,
        IUserService userService,
        IMapboxService mapboxService, 
        IStorageService storageService,
        IEventRepository eventRepository,
        ILogger<Event> logger
        )
    {
        _establishmentService = establishmentService;
        _userService = userService;
        _mapboxService = mapboxService;
        _storageService = storageService;
        _eventRepository = eventRepository;
        _logger = logger;
    }
    
    public async Task<EventDto> CreateEventAsync(string auth0Id, EventCreateRequest eventCreateRequest)
    {
        var establishment = await _establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id);
        
        var addressParts = eventCreateRequest.Address.Split(',');
        if (addressParts.Length != 2)
        {
            _logger.LogWarning("Address {Address} does not contain street and postal code separated by a comma", eventCreateRequest.Address);
            throw new ArgumentException("Address must contain street and postal code separated by a comma");
        }
        var street = addressParts[0].Trim();
        var postalCode = addressParts[1].Trim();
        var coordinates = await _mapboxService.GetCoordinatesFromAddressAsync(street, postalCode, eventCreateRequest.City);
        
        var eventEntity = new Event
        {
            Title = eventCreateRequest.Title,
            Body = eventCreateRequest.Body,
            Price = eventCreateRequest.Price,
            EventPriceCategory = eventCreateRequest.EventPriceCategory,
            Latitude = coordinates.lat,
            Longitude = coordinates.lng,
            Address = eventCreateRequest.Address,
            City = eventCreateRequest.City,
            MaxParticipants = eventCreateRequest.MaxParticipants,
            Start = eventCreateRequest.Start,
            End = eventCreateRequest.End,
            EventCategory = Enum.Parse<EventCategory>(eventCreateRequest.EventCategory, true),
            EstablishmentId = establishment.Id
        };
        
        await _eventRepository.CreateAsync(eventEntity);
        
        if (eventCreateRequest.Picture is not null)
        {
            eventEntity.PictureUrl = await _storageService.UploadPictureAsync(eventCreateRequest.Picture, "Establishment", auth0Id, $"event/{eventEntity.Id}");
            await _eventRepository.UpdateAsync(eventEntity);
        }

        return eventEntity.ToDto();
    }
    
    public async Task ReactToEventAsync(int eventId, string auth0Id)
    {
        var eventEntity = await _eventRepository.GetAsync(
            e => e.Id == eventId, 
            true, 
            e => e.Include(x => x.InterestedUsers));
        
        if (eventEntity.InterestedUsers.Any(iu => iu.Auth0Id == auth0Id))
        {
            eventEntity.InterestedUsers.Remove(eventEntity.InterestedUsers.First(iu => iu.Auth0Id == auth0Id));
        }
        else
        {
            eventEntity.InterestedUsers.Add(await _userService.GetUserByAuth0IdAsync(auth0Id));
        }
        
        await _eventRepository.UpdateAsync(eventEntity);
    }

    public async Task DeleteEventAsync(string auth0Id, int eventId)
    {
        var eventEntity = await _eventRepository.GetAsync(
            e => e.Id == eventId, 
            false, 
            e => e.Include(x => x.Establishment));

        if (eventEntity.Establishment.Auth0Id != auth0Id)
        {
            _logger.LogWarning("Establishment {Auth0Id} tried to delete event {EventId} that does not belong to them", auth0Id, eventId);
            throw new UnauthorizedAccessException("Establishment does not own this event");
        }
        
        await _eventRepository.DeleteAsync(eventId);
        
        if (eventEntity.PictureUrl is not null)
        {
            await _storageService.DeleteAsync("Establishment", auth0Id, $"event/{eventId}");
        }
    }
}