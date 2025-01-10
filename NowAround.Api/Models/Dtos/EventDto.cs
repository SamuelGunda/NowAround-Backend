using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Dtos;

public sealed record EventDto(
    int Id,
    string CreatorAuth0Id,
    string Title,
    string Body,
    double? Price,
    string City,
    string Address,
    double Latitude,
    double Longitude,
    string MaxParticipants,
    string? PictureUrl,
    DateTime Start,
    DateTime End,
    EventCategory EventCategory,
    DateTime CreatedAt,
    List<string> InterestedUsers
);