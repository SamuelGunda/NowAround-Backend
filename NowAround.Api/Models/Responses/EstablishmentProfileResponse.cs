using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Models.Responses;

public sealed record EstablishmentProfileResponse
(
    string Auth0Id,
    GenericInfo GenericInfo,
    LocationInfo LocationInfo,
    List<PostDto> Posts,
    List<MenuDto> Menus,
    List<EventDto> Events,
    RatingStatisticResponse RatingStatistic
);

public sealed record GenericInfo
(
    string Name,
    string ProfilePictureUrl,
    string BackgroundPictureUrl,
    string Description,
    string Website,
    string PriceRange,
    List<string> Tags,
    List<string> Categories,
    List<SocialLinkDto> SocialLinks
);

public sealed record LocationInfo
(
    string Address,
    string City,
    double Long,
    double Lat,
    BusinessHoursDto BusinessHours
);