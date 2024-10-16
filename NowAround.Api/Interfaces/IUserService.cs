namespace NowAround.Api.Interfaces;

public interface IUserService
{
    Task<int> CreateUserAsync(string auth0Id);
    
}