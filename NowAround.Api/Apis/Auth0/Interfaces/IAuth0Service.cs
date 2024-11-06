using NowAround.Api.Apis.Auth0.Models.Requests;

namespace NowAround.Api.Apis.Auth0.Interfaces;

public interface IAuth0Service
{
    // Establishment account functions
    Task<string> RegisterEstablishmentAccountAsync(PersonalInfo personalInfo);
    Task<string> GetEstablishmentOwnerFullNameAsync(string auth0Id);
    
    // User account functions
    
    // General account functions
    Task<int> GetRegisteredAccountsCountByMonthAndRoleAsync(DateTime date, string role);
    Task DeleteAccountAsync(string auth0Id);
    Task AssignRoleToAccountAsync(string auth0Id, string role);
}