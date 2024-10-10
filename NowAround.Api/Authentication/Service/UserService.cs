using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Authentication.Service;

public class UserService(AppDbContext context) : IUserService
{
    
    public async Task<int> CreateUserAsync(string auth0Id)
    {
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