using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories;

public class MonthlyStatisticRepository : IMonthlyStatisticRepository
{
    
    private readonly AppDbContext _context;
    private readonly ILogger<MonthlyStatisticRepository> _logger;

    public MonthlyStatisticRepository(AppDbContext context, ILogger<MonthlyStatisticRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new monthly statistic.
    /// </summary>
    /// <param name="monthlyStatistic"> The monthly statistic to be created </param>
    /// <returns> The task representing the asynchronous operation, containing the number of state entries written to the database </returns>
    /// <exception cref="Exception"> Thrown when there is an error creating the monthly statistic </exception>
    public async Task CreateMonthlyStatisticAsync(MonthlyStatistic monthlyStatistic)
    {
        try
        {
            await _context.MonthlyStatistics.AddAsync(monthlyStatistic);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to create monthly statistic: {Message}", e.Message);
            throw new Exception("Failed to create monthly statistic", e);
        }
    }

    /// <summary>
    /// Retrieves the monthly statistics for a specific date.
    /// </summary>
    /// <param name="date"> The date to filter the monthly statistics by </param>
    /// <returns> The monthly statistics for the specified date, or null if not found </returns>
    /// <exception cref="Exception"> Thrown when there is an error retrieving the monthly statistics </exception>
    public async Task<MonthlyStatistic?> GetMonthlyStatisticByDateAsync(string date)
    {
        try
        {
            var monthlyStatistics = await _context.MonthlyStatistics
                .Where(ms => ms.Date == date)
                .FirstOrDefaultAsync();
            return monthlyStatistics;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get monthly statistics by date: {Message}", e.Message);
            throw new Exception("Failed to get monthly statistics by date", e);
        }
    }
}