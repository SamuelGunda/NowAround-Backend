namespace NowAround.Application.Dtos;

public sealed record EventDto(
    int Id,
    string CreatorAuth0Id,
    string Title,
    string Body,
    string? Price,
    string? EventPriceCategory,
    string City,
    string Address,
    double Latitude,
    double Longitude,
    string? MaxParticipants,
    string? PictureUrl,
    DateTime Start,
    DateTime End,
    string EventCategory,
    DateTime CreatedAt,
    List<string> InterestedUsers
);