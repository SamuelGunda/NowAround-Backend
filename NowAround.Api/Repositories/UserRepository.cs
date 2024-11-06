using NowAround.Api.Database;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories;

public class UserRepository : IUserRepository
{
    
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;
    
    public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    } 
    
    /// <summary>
    /// Creates a new user in the database.
    /// </summary>
    /// <param name="user">The user entity to be created.</param>
    /// <returns>The ID of the newly created user.</returns>
    /// <exception cref="Exception">Thrown when database fails.</exception>
    public async Task<int> CreateUserAsync(User user)
    {
        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create user");
            throw new Exception("Failed to create user", e);
        }
    }
}