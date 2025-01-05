using NowAround.Api.Models.Entities;
using NowAround.Api.Repositories.Interfaces;

namespace NowAround.Api.Repositories;

public interface IBaseAccountRepository<T> : IBaseRepository<T> where T : BaseAccountEntity
{
    Task<T?> GetByAuth0IdAsync(string auth0Id);
    Task<int> GetCountByCreatedAtBetweenDatesAsync(DateTime startDate, DateTime endDate);
    Task<bool> DeleteByAuth0IdAsync(string auth0Id);
}