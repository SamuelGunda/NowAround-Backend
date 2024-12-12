using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Models.Responses;

public sealed record EstablishmentProfileResponse(
    string Auth0Id,
    GenericInfo GenericInformation,
    LocationInfo LocationInfo,
    List<PostWithAuthIdsResponse> Posts,
    List<MenuDto> Menus,
    RatingStatisticResponse RatingStatistic
);

public sealed record GenericInfo(
    string Name,
    string Photo,
    List<string> Tags,
    List<string> Categories,
    string PriceRange,
    List<string> Cuisine,
    List<SocialLinkDto> SocialLinks
);

public sealed record LocationInfo(
    string Address,
    string City,
    BusinessHoursDto BusinessHours,
    double Long,
    double Lat
);