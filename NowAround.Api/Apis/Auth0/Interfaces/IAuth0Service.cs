using NowAround.Api.Apis.Auth0.Models.Requests;

namespace NowAround.Api.Apis.Auth0.Interfaces;

public interface IAuth0Service
{
    Task<string> RegisterEstablishmentAccountAsync(PersonalInfo personalInfo);
    Task DeleteAccountAsync(string auth0Id);
}