using NowAround.Api.Apis.Auth0.Models.Requests;

namespace NowAround.Api.Apis.Auth0.Interfaces;

public interface IAuth0Service
{
    // Establishment account functions
    Task<string> RegisterEstablishmentAccountAsync(OwnerInfo ownerInfo);
    Task<string> GetEstablishmentOwnerFullNameAsync(string auth0Id);
    
    Task DeleteAccountAsync(string auth0Id);
    Task AssignRoleAsync(string auth0Id, string role);
}