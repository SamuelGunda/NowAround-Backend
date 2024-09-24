namespace NowAround.Api.Authentication.Interfaces;

public interface IAccountService
{
    Task<string> CheckIfAccountExistAsync(int auth0Id);
}