using NowAround.Api.Apis.Mapbox.Interfaces;
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
    private readonly IMapboxService _mapboxService;
    private readonly IStorageService _storageService;
    private readonly IEventRepository _eventRepository;

    public EventService(
        IEstablishmentService establishmentService,
        IMapboxService mapboxService, 
        IStorageService storageService,
        IEventRepository eventRepository
        )
    {
        _establishmentService = establishmentService;
        _mapboxService = mapboxService;
        _storageService = storageService;
        _eventRepository = eventRepository;
    }
    
    public async Task<EventDto> CreateEventAsync(string auth0Id, EventCreateRequest eventCreateRequest)
    {
        var establishment = await _establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id);
        
        var addressParts = eventCreateRequest.Address.Split(',');
        if (addressParts.Length != 2)
        {
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

    public Task DeleteEventAsync(string auth0Id, int eventId)
    {
        throw new NotImplementedException();
    }
}