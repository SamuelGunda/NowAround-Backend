using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Models.Responses;

public sealed record EstablishmentProfileResponse(
    string Auth0Id,
    GenericInfo GenericInfo,
    LocationInfo LocationInfo,
    List<PostWithAuthIdsResponse> Posts,
    List<MenuDto> Menus,
    RatingStatisticResponse RatingStatistic
);

public sealed record GenericInfo(
    string Name,
    string Photo,
    string Description,
    string Website,
    string PriceRange,
    List<string> Tags,
    List<string> Categories,
    List<string> Cuisine,
    List<SocialLinkDto> SocialLinks
);

public sealed record LocationInfo(
    string Address,
    string City,
    double Long,
    double Lat,
    BusinessHoursDto BusinessHours
);