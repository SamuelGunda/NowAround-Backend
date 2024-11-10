using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Models.Responses;

namespace NowAround.Api.Interfaces;

public interface IEstablishmentService
{
    Task RegisterEstablishmentAsync(EstablishmentRegisterRequest request);
    Task<EstablishmentResponse> GetEstablishmentByIdAsync(int id);
    Task<EstablishmentResponse> GetEstablishmentByAuth0IdAsync(string auth0Id);
    Task<List<PendingEstablishmentResponse>?> GetPendingEstablishmentsAsync();
    Task<List<EstablishmentResponse>?> GetEstablishmentMarkersWithFilterAsync(string? name, string? categoryName, List<string>? tagNames);
    Task<List<EstablishmentResponse>?> GetEstablishmentMarkersWithFilterInAreaAsync(MapBounds mapBounds, string? name, string? categoryName, List<string>? tagNames);
    Task<int> GetEstablishmentsCountCreatedInMonthAsync(DateTime monthStart, DateTime monthEnd);
    Task UpdateEstablishmentAsync(EstablishmentUpdateRequest request);
    Task UpdateEstablishmentRegisterRequestAsync(string auth0Id, RequestStatus requestStatus);
    Task DeleteEstablishmentAsync(string auth0Id);
}