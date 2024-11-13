using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories;

public interface IUserRepository : IBaseAccountRepository<User>
{
    Task<int> GetUsersCountByCreatedAtBetweenDatesAsync(DateTime startDate, DateTime endDate);
}

public class UserRepository : BaseAccountRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, ILogger<User> logger) 
        : base(context, logger)
    {
    }

    /// <summary>
    /// Gets the count of users created between the specified start and end dates.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>The count of users created between the specified dates.</returns>
    public async Task<int> GetUsersCountByCreatedAtBetweenDatesAsync(DateTime startDate, DateTime endDate)
    {
        return await GetCountByCreatedAtBetweenDatesAsync(startDate, endDate);
    }
}