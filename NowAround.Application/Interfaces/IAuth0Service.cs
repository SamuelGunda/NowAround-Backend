using NowAround.Application.Dtos;
using NowAround.Application.Requests;

namespace NowAround.Application.Interfaces;

public interface IAuth0Service
{
    // Establishment account functions
    Task<string> RegisterEstablishmentAccountAsync(EstablishmentOwnerInfo establishmentOwnerInfo);
    Task<(string fullName, string email)> GetEstablishmentOwnerFullNameAndEmailAsync(string auth0Id);
    Task ChangeAccountPasswordAsync(string auth0Id,  string newPassword);
    Task<bool> VerifyOldPasswordAsync(string auth0Id, string oldPassword);
    Task DeleteAccountAsync(string auth0Id);
    Task AssignRoleAsync(string auth0Id, string role);
}