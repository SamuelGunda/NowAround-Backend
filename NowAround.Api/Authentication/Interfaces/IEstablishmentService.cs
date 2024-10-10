using NowAround.Api.Authentication.Models;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Authentication.Interfaces;

public interface IEstablishmentService
{
    Task<EstablishmentDto> GetEstablishmentAsync(string auth0Id);
    /*Task<IEnumerable<Establishment>> GetEstablishmentsAsync();*/
    Task<int> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest);
    /*Task UpdateEstablishmentAsync(Establishment establishment);*/
    Task<bool> DeleteEstablishmentAsync(string auth0Id);
}