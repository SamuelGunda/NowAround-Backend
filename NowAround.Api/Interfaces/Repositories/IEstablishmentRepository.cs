using NowAround.Api.Models.Domain;

namespace NowAround.Api.Interfaces.Repositories;

public interface IEstablishmentRepository
{
    Task<bool> CheckIfEstablishmentExistsByNameAsync(string name);
    Task<int> CreateEstablishmentAsync(Establishment establishment);
    Task<Establishment?> GetEstablishmentByIdAsync(int id);
    Task<bool> DeleteEstablishmentByAuth0IdAsync(string auth0Id);   
}