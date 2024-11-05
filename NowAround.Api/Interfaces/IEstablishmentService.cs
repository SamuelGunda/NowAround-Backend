using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Models.Responses;

namespace NowAround.Api.Interfaces;

public interface IEstablishmentService
{
    Task<int> RegisterEstablishmentAsync(EstablishmentRegisterRequest request);
    //Task<EstablishmentDto> GetEstablishmentByIdAsync(int id);
    Task<EstablishmentResponse> GetEstablishmentByAuth0IdAsync(string auth0Id);
    Task<List<EstablishmentResponse>?> GetPendingEstablishmentsAsync();
    Task<List<EstablishmentResponse>?> GetEstablishmentMarkersWithFilterAsync(string? name, string? categoryName, List<string>? tagNames);
    Task<List<EstablishmentResponse>?> GetEstablishmentMarkersWithFilterInAreaAsync(MapBounds mapBounds, string? name, string? categoryName, List<string>? tagNames);
    Task UpdateEstablishmentAsync(EstablishmentUpdateRequest request);
    Task UpdateEstablishmentRegisterRequestAsync(string auth0Id, RequestStatus requestStatus);
    Task DeleteEstablishmentAsync(string auth0Id);
}