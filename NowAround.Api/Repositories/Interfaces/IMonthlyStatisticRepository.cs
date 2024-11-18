using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories.Interfaces;

public interface IMonthlyStatisticRepository
{
    Task CreateMonthlyStatisticAsync(MonthlyStatistic monthlyStatistic);
    Task<MonthlyStatistic?> GetMonthlyStatisticByDateAsync(string date);
}