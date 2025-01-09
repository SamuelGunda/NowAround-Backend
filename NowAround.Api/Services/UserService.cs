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
    private readonly IStorageService _storageService;
    public UserService(ILogger<UserService> logger, IBaseAccountRepository<User> userRepository, IAuth0Service auth0Service, IStorageService storageService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _auth0Service = auth0Service;
        _storageService = storageService;
    }
    
    public async Task CreateUserAsync(string auth0Id, string fullName)
    {
        var user = new User { Auth0Id = auth0Id, FullName = fullName };
        
        await _auth0Service.AssignRoleAsync(auth0Id, "user");
        
        await _userRepository.CreateAsync(user);
    }

    /*
    public Task<bool> CheckIfUserExistsAsync(string auth0Id)
    {
        return _userRepository.CheckIfExistsByPropertyAsync("Auth0Id", auth0Id);
    }
    */

    public async Task<User> GetUserByAuth0IdAsync(string auth0Id, bool tracked = false)
    {
        var user = await _userRepository.GetAsync(u => u.Auth0Id == auth0Id, tracked);
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

    public async Task UpdateUserPictureAsync(string auth0Id, string pictureContext, IFormFile picture)
    {
        if (pictureContext != "profile-picture" && pictureContext != "background-picture")
        {
            _logger.LogWarning("Invalid image context: {ImageContext}", pictureContext);
            throw new ArgumentException("Invalid image context", nameof(pictureContext));
        }
        
        var pictureType = picture.ContentType;
        _storageService.CheckPictureType(pictureType);
        
        var user = await GetUserByAuth0IdAsync(auth0Id, true);
        var pictureUrl = await _storageService.UploadPictureAsync(picture, "User", auth0Id, pictureContext, null);
        
        user.ProfilePictureUrl = pictureUrl.Contains("profile-picture") ? pictureUrl : user.ProfilePictureUrl;
        user.BackgroundPictureUrl = pictureUrl.Contains("background-picture") ? pictureUrl : user.BackgroundPictureUrl;
        
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(string auth0Id)
    {
        await _auth0Service.DeleteAccountAsync(auth0Id);
        
        await _storageService.DeleteAsync("User", auth0Id);
        
        //TODO: Get whole user object and delete it
        var result = await _userRepository.DeleteByAuth0IdAsync(auth0Id);
        if (!result)
        {
            _logger.LogError("Failed to delete user with Auth0 ID: {Auth0Id}", auth0Id);
            throw new EntityNotFoundException("User", "Auth0 ID", auth0Id);
        }
    }
}