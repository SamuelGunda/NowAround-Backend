using NowAround.Api.Models.Domain;

namespace NowAround.Api.Authentication.Interfaces;

public interface IEstablishmentRepository
{
    Task<bool> CheckIfEstablishmentExistsByNameAsync(string name);
    Task<int> CreateEstablishmentAsync(Establishment establishment);
    Task<Establishment> GetEstablishmentByAuth0IdAsync(string auth0Id);
    Task<bool> DeleteEstablishmentByAuth0IdAsync(string auth0Id);
    
    
}