using NowAround.Api.Apis.Auth0.Models;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Requests;

namespace NowAround.Api.Interfaces;

public interface IEstablishmentService
{
    Task<EstablishmentDto> GetEstablishmentByIdAsync(int id);
    Task<EstablishmentDto> GetEstablishmentByAuth0IdAsync(string auth0Id);
    /*Task<IEnumerable<Establishment>> GetEstablishmentsAsync();*/
    Task<List<EstablishmentPin>?> GetEstablishmentPinsByAreaAsync(EstablishmentsInAreaRequest establishmentsInAreaRequest);
    Task<int> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest);
    /*Task UpdateEstablishmentAsync(Establishment establishment);*/
    Task DeleteEstablishmentAsync(string auth0Id);
}