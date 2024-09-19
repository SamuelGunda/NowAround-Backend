namespace NowAround.Api.Authentication.Interfaces;

public interface ITokenService
{
    Task<string> GetManagementAccessTokenAsync();
}