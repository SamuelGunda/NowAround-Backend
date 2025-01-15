using NowAround.Application.Responses;
using NowAround.Domain.Models;

namespace NowAround.Application.Mapping;

public static class EstablishmentMarkerResponseMapper
{
    public static EstablishmentMarkerResponse ToMarkerResponse(this Establishment establishment)
    {
        ArgumentNullException.ThrowIfNull(establishment, nameof(establishment));

        return new EstablishmentMarkerResponse(
            establishment.Auth0Id,
            establishment.Name,
            establishment.Description,
            establishment.PriceCategory.ToString(),
            establishment.Tags.Select(t => t.Name).ToList(),
            establishment.Longitude,
            establishment.Latitude
        );
    }
}