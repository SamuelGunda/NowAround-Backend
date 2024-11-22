using NowAround.Api.Models.Domain;

namespace NowAround.Api.Services.Interfaces;

public interface IUserService
{
    Task CreateUserAsync(string auth0Id, string fullName);
    Task<User?> GetUserAsync(string auth0Id);
    Task<int> GetUsersCountCreatedInMonthAsync(DateTime startDate, DateTime endDate);
}