using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Interfaces;

public interface IEstablishmentService
{
    Task<EstablishmentDto> GetEstablishmentByIdAsync(int id);
    Task<EstablishmentDto> GetEstablishmentByAuth0IdAsync(string auth0Id);
    Task<List<EstablishmentPin>?> GetEstablishmentPinsByAreaAsync(MapBounds mapBounds);
    Task<List<EstablishmentPin>?> GetEstablishmentPinsWithFilterByAreaAsync(MapBounds mapBounds, string? name, string? categoryName, List<string>? tagNames);
    Task<int> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest);
    /*Task UpdateEstablishmentAsync(Establishment establishment);*/
    Task DeleteEstablishmentAsync(string auth0Id);
}