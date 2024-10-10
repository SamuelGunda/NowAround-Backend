using NowAround.Api.Models.Domain;

namespace NowAround.Api.Authentication.Interfaces;

public interface IEstablishmentRepository
{
    Task<bool> CheckIfEstablishmentExistsByNameAsync(string name);
    Task<int> CreateEstablishmentAsync(Establishment establishment);
}