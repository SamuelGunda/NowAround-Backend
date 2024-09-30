using NowAround.Api.Authentication.Models;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Authentication.Interfaces;

public interface IEstablishmentService
{
    /*Task<Establishment> GetEstablishmentAsync(int id);
    Task<IEnumerable<Establishment>> GetEstablishmentsAsync();*/
    Task<int> CreateEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest);
    /*Task UpdateEstablishmentAsync(Establishment establishment);
    Task DeleteEstablishmentAsync(int id);*/
}