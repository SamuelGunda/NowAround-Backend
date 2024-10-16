using NowAround.Api.Apis.Auth0.Models;
using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Interfaces;

public interface IEstablishmentService
{
    Task<EstablishmentDto> GetEstablishmentAsync(string auth0Id);
    /*Task<IEnumerable<Establishment>> GetEstablishmentsAsync();*/
    Task<int> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest);
    /*Task UpdateEstablishmentAsync(Establishment establishment);*/
    Task<bool> DeleteEstablishmentAsync(string auth0Id);
}