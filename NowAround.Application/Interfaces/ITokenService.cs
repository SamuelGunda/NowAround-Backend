namespace NowAround.Application.Interfaces;

public interface ITokenService
{
    Task<string> GetManagementAccessTokenAsync();
}