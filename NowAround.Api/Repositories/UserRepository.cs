using Microsoft.EntityFrameworkCore;
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

    /// <summary>
    /// Gets the count of users created between the specified start and end dates.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>The count of users created between the specified dates.</returns>
    /// <exception cref="Exception">Thrown when there is an error retrieving the count of users.</exception>
    public async Task<int> GetUsersCountByCreatedAtBetweenDatesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            return await _context.Users
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .CountAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get users count");
            throw new Exception("Failed to get users count", e);
        }
    }
}