using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Services;

public class UserService : IUserService
{
    
    private readonly ILogger<UserService> _logger;
    private readonly IBaseAccountRepository<User> _userRepository;
    private readonly IAuth0Service _auth0Service;
    public UserService(ILogger<UserService> logger, IBaseAccountRepository<User> userRepository, IAuth0Service auth0Service)
    {
        _logger = logger;
        _userRepository = userRepository;
        _auth0Service = auth0Service;
    }
    
    public async Task CreateUserAsync(string auth0Id)
    {
        var user = new User { Auth0Id = auth0Id };
        
        await _auth0Service.AssignRoleAsync(auth0Id, "user");
        await _userRepository.CreateAsync(user);
    }
    
    public async Task<User?> GetUserAsync(string auth0Id)
    {
        var user = await _userRepository.GetByAuth0IdAsync(auth0Id);
        
        if (user == null)
        {
            _logger.LogWarning("User with Auth0 ID: {Auth0Id} not found", auth0Id);
            throw new EntityNotFoundException("User", "Auth0 ID", auth0Id);
        }
        
        return user;
    }
    
    public async Task<int> GetUsersCountCreatedInMonthAsync(DateTime startDate, DateTime endDate)
    {
        return await _userRepository.GetCountByCreatedAtBetweenDatesAsync(startDate, endDate);
    }
}