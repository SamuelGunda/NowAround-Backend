namespace NowAround.Api.Interfaces;

public interface IUserService
{
    Task CreateUserAsync(string auth0Id);
    
}