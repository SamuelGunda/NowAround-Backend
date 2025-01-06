namespace NowAround.Api.Apis.Auth0.Interfaces;

public interface ITokenService
{
    Task<string> GetManagementAccessTokenAsync();
}