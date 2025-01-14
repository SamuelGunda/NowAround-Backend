using NowAround.Application.Dtos;
using NowAround.Domain.Models;

namespace NowAround.Application.Mapping;

public static class EventMapper
{
    public static EventDto ToDto(this Event eventEntity)
    {
        ArgumentNullException.ThrowIfNull(eventEntity, nameof(eventEntity));

        return new EventDto(
            Id: eventEntity.Id,
            CreatorAuth0Id: eventEntity.Establishment?.Auth0Id ?? null,
            Title: eventEntity.Title,
            Body: eventEntity.Body,
            Price: eventEntity.Price,
            EventPriceCategory: eventEntity.EventPriceCategory,
            City: eventEntity.City,
            Address: eventEntity.Address,
            Latitude: eventEntity.Latitude,
            Longitude: eventEntity.Longitude,
            MaxParticipants: eventEntity.MaxParticipants,
            PictureUrl: eventEntity.PictureUrl,
            Start: eventEntity.Start,
            End: eventEntity.End,
            EventCategory: eventEntity.EventCategory.ToString(),
            CreatedAt: eventEntity.CreatedAt,
            InterestedUsers: eventEntity.InterestedUsers.Select(x => x.Auth0Id).ToList()
        );
    }
}