namespace NowAround.Api.Interfaces;

public interface IUserService
{
    Task CreateUserAsync(string auth0Id);

    Task<int> GetUsersCountCreatedInMonthAsync(DateTime startDate, DateTime endDate);
}