using Microsoft.IdentityModel.Tokens;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Database;
using NowAround.Api.Interfaces;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;
using NowAround.Api.Repositories;

namespace NowAround.Api.Services;

public interface IUserService
{
    Task CreateUserAsync(string auth0Id);
    Task<User?> GetUserAsync(string auth0Id);
    Task<int> GetUsersCountCreatedInMonthAsync(DateTime startDate, DateTime endDate);
}

public class UserService : IUserService
{
    
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IAuth0Service _auth0Service;
    public UserService(ILogger<UserService> logger, IUserRepository userRepository, IAuth0Service auth0Service)
    {
        _logger = logger;
        _userRepository = userRepository;
        _auth0Service = auth0Service;
    }
    
    /// <summary>
    /// Assign user to azure database and assign user role.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the user to be created </param>
    /// <returns> A task that represents the asynchronous operation </returns>
    /// <exception cref="Exception"> Thrown when the user creation or role assignment fails </exception>
    public async Task CreateUserAsync(string auth0Id)
    {
        var user = new User { Auth0Id = auth0Id };
        
        try
        {
            await _userRepository.CreateAsync(user);
            await _auth0Service.AssignRoleAsync(auth0Id, "user");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create user");
            throw new Exception("Failed to create user", e);
        }
    }
    
    public async Task<User?> GetUserAsync(string auth0Id)
    {
        var user = await _userRepository.GetByAuth0IdAsync(auth0Id);
        
        if (user == null)
        {
            _logger.LogWarning("User with Auth0 ID {Auth0Id} not found", auth0Id);
            throw new Exception("User not found");
        }
        
        return user;
    }

    /// <summary>
    /// Retrieves the count of users created within a specified date range.
    /// </summary>
    /// <param name="startDate"> The start date of the date range </param>
    /// <param name="endDate"> The end date of the date range </param>
    /// <returns> The task result contains the count of users created within the specified date range </returns>
    public async Task<int> GetUsersCountCreatedInMonthAsync(DateTime startDate, DateTime endDate)
    {
        return await _userRepository.GetCountByCreatedAtBetweenDatesAsync(startDate, endDate);
    }
}