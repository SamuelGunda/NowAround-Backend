using NowAround.Api.Apis.Auth0.Models;
using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Interfaces;

public interface IEstablishmentService
{
    Task<EstablishmentDto> GetEstablishmentByIdAsync(int id);
    /*Task<IEnumerable<Establishment>> GetEstablishmentsAsync();*/
    Task<int> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest);
    /*Task UpdateEstablishmentAsync(Establishment establishment);*/
    Task DeleteEstablishmentAsync(string auth0Id);
}