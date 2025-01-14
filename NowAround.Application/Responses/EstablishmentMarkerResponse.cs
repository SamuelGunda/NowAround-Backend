namespace NowAround.Application.Responses;

public sealed record EstablishmentMarkerResponse(
    string Auth0Id,
    string Name,
    string Description,
    string PriceCategory,
    List<string> Tags,
    double? Longitude,
    double? Latitude
);