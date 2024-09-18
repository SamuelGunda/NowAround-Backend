namespace NowAround.Api.Authorization.Interfaces;

public interface ITokenService
{
    Task<string> GetManagementAccessTokenAsync();
}