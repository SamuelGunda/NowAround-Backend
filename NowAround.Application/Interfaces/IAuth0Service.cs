using NowAround.Application.Dtos;
using NowAround.Application.Requests;

namespace NowAround.Application.Interfaces;

public interface IAuth0Service
{
    // Establishment account functions
    Task<string> RegisterEstablishmentAccountAsync(EstablishmentOwnerInfo establishmentOwnerInfo);
    Task<string> GetEstablishmentOwnerFullNameAsync(string auth0Id);
    
    Task DeleteAccountAsync(string auth0Id);
    Task AssignRoleAsync(string auth0Id, string role);
}