using NowAround.Application.Dtos;

namespace NowAround.Application.Interfaces;

public interface IAuth0Service
{
    // Establishment account functions
    Task<string> RegisterEstablishmentAccountAsync(OwnerInfo ownerInfo);
    Task<string> GetEstablishmentOwnerFullNameAsync(string auth0Id);
    
    Task DeleteAccountAsync(string auth0Id);
    Task AssignRoleAsync(string auth0Id, string role);
}