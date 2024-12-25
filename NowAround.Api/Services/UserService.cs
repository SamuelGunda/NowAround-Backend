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
    
    public async Task CreateUserAsync(string auth0Id, string fullName)
    {
        var user = new User { Auth0Id = auth0Id, FullName = fullName };
        
        await _auth0Service.AssignRoleAsync(auth0Id, "user");
        await _userRepository.CreateAsync(user);
    }

    public Task<bool> CheckIfUserExistsAsync(string auth0Id)
    {
        return _userRepository.CheckIfExistsByPropertyAsync("Auth0Id", auth0Id);
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
    
    public async Task<int> GetUsersCountCreatedInMonthAsync(DateTime monthStart, DateTime monthEnd)
    {
        return await _userRepository.GetCountByCreatedAtBetweenDatesAsync(monthStart, monthEnd);
    }

    public async Task UpdateUserPictureAsync(string auth0Id, string imageUrl)
    {
        var user = await _userRepository.GetByAuth0IdAsync(auth0Id);
        
        if (user == null)
        {
            _logger.LogWarning("User with Auth0 ID: {Auth0Id} not found", auth0Id);
            throw new EntityNotFoundException("User", "Auth0 ID", auth0Id);
        }
        
        user.ProfilePictureUrl = imageUrl.Contains("profile-picture") ? imageUrl : user.ProfilePictureUrl;
        user.BackgroundPictureUrl = !imageUrl.Contains("profile-picture") ? imageUrl : user.BackgroundPictureUrl;
        
        await _userRepository.UpdateAsync(user);
    }
}