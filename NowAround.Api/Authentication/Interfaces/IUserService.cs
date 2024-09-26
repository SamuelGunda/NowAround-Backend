namespace NowAround.Api.Authentication.Interfaces;

public interface IUserService
{
    Task<int> CreateUserAsync(string auth0Id);
}