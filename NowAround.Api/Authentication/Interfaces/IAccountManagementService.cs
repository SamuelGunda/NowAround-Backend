using NowAround.Api.Authentication.Models;

namespace NowAround.Api.Authentication.Interfaces;

public interface IAccountManagementService
{
    Task<string> RegisterEstablishmentAccountAsync(string establishmentName, PersonalInfo personalInfo);
    Task<bool> DeleteAccountAsync(string auth0Id);
}