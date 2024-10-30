using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Interfaces;

public interface IEstablishmentService
{
    Task<int> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest);
    Task<EstablishmentDto> GetEstablishmentByIdAsync(int id);
    Task<EstablishmentDto> GetEstablishmentByAuth0IdAsync(string auth0Id);
    Task<List<EstablishmentDto>?> GetPendingEstablishmentsAsync();
    Task<List<EstablishmentPin>?> GetEstablishmentPinsInAreaAsync(MapBounds mapBounds);
    Task<List<EstablishmentPin>?> GetEstablishmentPinsWithFilterInAreaAsync(MapBounds mapBounds, string? name, string? categoryName, List<string>? tagNames);
    //TODO: Task UpdateEstablishmentAsync(Establishment establishment);
    Task UpdateEstablishmentRegisterRequestAsync(string auth0Id, RequestStatus requestStatus);
    Task DeleteEstablishmentAsync(string auth0Id);
}