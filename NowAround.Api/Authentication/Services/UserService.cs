using Microsoft.IdentityModel.Tokens;
using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Authentication.Services;

public class UserService(AppDbContext context, ILogger<UserService> logger) : IUserService
{
    
    public async Task<int> CreateUserAsync(string auth0Id)
    {
        if (auth0Id.IsNullOrEmpty())
        {
            logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        var user = new User()
        {
            Auth0Id = auth0Id,
            Role = Role.User,
        };
        
        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to create user", e);
        }
    }
}