using Microsoft.Extensions.Logging;
using NowAround.Application.Common.Helpers;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;

namespace NowAround.Application.Services;

public class MonthlyStatisticService : IMonthlyStatisticService
{
    
    private readonly IMonthlyStatisticRepository _monthlyStatisticRepository;
    private readonly IEstablishmentService _establishmentService;
    private readonly IUserService _userService;
    private readonly ILogger<MonthlyStatisticService> _logger;
    
    public MonthlyStatisticService(
        IMonthlyStatisticRepository monthlyStatisticRepository, 
        IEstablishmentService establishmentService, 
        IUserService userService, 
        ILogger<MonthlyStatisticService> logger)
    {
        _monthlyStatisticRepository = monthlyStatisticRepository;
        _establishmentService = establishmentService;
        _userService = userService;
        _logger = logger;
    }
    
    public async Task<List<MonthlyStatistic>> GetMonthlyStatisticByYearAsync(string year)
    {
        var yearCheck = year + "-01-01";
        
        if (!DateTime.TryParse(yearCheck, out _))
        {
            _logger.LogError("Year is not in the correct format");
            throw new ArgumentException("Year is not in the correct format");
        }
        
        if (DateTime.Parse(yearCheck) > DateTime.Now)
        {
            _logger.LogError("Year cannot be in the future");
            return [];
        }

        var months = GetMonthsInYear(year);
        var yearsStatistics = new List<MonthlyStatistic>();
        
        foreach (var month in months)
        {
            // Check if the month is in the future and add it to the list with 0 values
            if (DateTime.Parse(month).AddMonths(1).AddDays(-1) > DateTime.Now)
            {
                yearsStatistics.Add(new MonthlyStatistic
                {
                    Date = month,
                    UsersCreatedCount = 0,
                    EstablishmentsCreatedCount = 0
                });
                continue;
            }
            
            var statistics = await _monthlyStatisticRepository.GetMonthlyStatisticByDateAsync(month);
            
            if (statistics != null)
            {
                yearsStatistics.Add(statistics);
            }
            else
            {
               var monthStartAndEnd = GetMonthStartAndEndDate(month); 
               
               yearsStatistics.Add(await CreateMonthlyStatistic(month, monthStartAndEnd.StartDate, monthStartAndEnd.EndDate));
            }
        }
        
        return yearsStatistics;
    }

    private async Task<MonthlyStatistic> CreateMonthlyStatistic(string month, DateTime startDate, DateTime endDate)
    {
        var establishmentsCount = await _establishmentService.GetEstablishmentsCountCreatedInMonthAsync(startDate, endDate);
        var userCount = await _userService.GetUsersCountCreatedInMonthAsync(startDate, endDate);
        
        var newStatistic = new MonthlyStatistic
        {
            Date = month,
            UsersCreatedCount = userCount,
            EstablishmentsCreatedCount = establishmentsCount
        };
        
        await _monthlyStatisticRepository.CreateMonthlyStatisticAsync(newStatistic);

        return newStatistic;
    }
    
    private static List<string> GetMonthsInYear(string year)
    {
        var start = DateTime.Parse(year + "-01");
        var end = DateTime.Parse(year + "-12");
        var months = new List<string>();

        while (start <= end)
        {
            months.Add(start.ToString("yyyy-MM"));
            start = start.AddMonths(1);
        }

        return months;
    }
    
    private static (DateTime StartDate, DateTime EndDate) GetMonthStartAndEndDate(string date)
    {
        var year = date.Split('-').Select(int.Parse).ToArray()[0];
        var month = date.Split('-').Select(int.Parse).ToArray()[1];
        
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return (startDate, endDate);
    }
}