using NowAround.Api.Models.Domain;

namespace NowAround.Api.Interfaces.Repositories;

public interface IUserOldRepository
{
    Task<int> CreateUserAsync(User user);
    Task<int> GetUsersCountByCreatedAtBetweenDatesAsync(DateTime startDate, DateTime endDate);
}