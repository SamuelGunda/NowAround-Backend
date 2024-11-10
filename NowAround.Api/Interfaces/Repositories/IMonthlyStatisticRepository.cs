using NowAround.Api.Models.Domain;

namespace NowAround.Api.Interfaces.Repositories;

public interface IMonthlyStatisticRepository
{
    Task CreateMonthlyStatisticAsync(MonthlyStatistic monthlyStatistic);
    Task<MonthlyStatistic?> GetMonthlyStatisticByDateAsync(string date);
}