using NowAround.Domain.Models;

namespace NowAround.Domain.Interfaces.Specific;

public interface IMonthlyStatisticRepository
{
    Task CreateMonthlyStatisticAsync(MonthlyStatistic monthlyStatistic);
    Task<MonthlyStatistic?> GetMonthlyStatisticByDateAsync(string date);
}