using NowAround.Api.Apis.Auth0.Models;

namespace NowAround.Api.Apis.Auth0.Interfaces;

public interface IAuth0Service
{
    Task<string> RegisterEstablishmentAccountAsync(string establishmentName, PersonalInfo personalInfo);
    Task<bool> DeleteAccountAsync(string auth0Id);
}