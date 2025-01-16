using NowAround.Domain.Models;

namespace NowAround.Application.Services;

public interface IMonthlyStatisticService
{
    Task<List<MonthlyStatistic>> GetMonthlyStatisticByYearAsync(string year);
}