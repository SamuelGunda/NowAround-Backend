using NowAround.Api.Models.Domain;

namespace NowAround.Api.Services.Interfaces;

public interface IUserService
{
    Task CreateUserAsync(string auth0Id, string fullName);
    /*Task<bool> CheckIfUserExistsAsync(string auth0Id);*/
    Task<User> GetUserByAuth0IdAsync(string auth0Id, bool tracked = false, params Func<IQueryable<User>, IQueryable<User>>[] includeProperties);
    Task<int> GetUsersCountCreatedInMonthAsync(DateTime monthStart, DateTime monthEnd);
    Task UpdateUserPictureAsync(string auth0Id, string pictureContext, IFormFile picture);
    Task DeleteUserAsync(string auth0Id);
}