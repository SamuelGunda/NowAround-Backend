using NowAround.Api.Models.Domain;

namespace NowAround.Api.Interfaces;

public interface IMonthlyStatisticService
{
    Task<List<MonthlyStatistic>> GetMonthlyStatisticByYearAsync(string year);
}