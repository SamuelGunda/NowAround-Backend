using NowAround.Api.Interfaces;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;
using NowAround.Api.Utilities;

namespace NowAround.Api.Services;

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
    
    /// <summary>
    /// Retrieves the monthly statistics for a specific year.
    /// </summary>
    /// <param name="year"> The year to filter the monthly statistics by </param>
    /// <returns> A list of monthly statistics for the specified year </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the year is not in the correct format or is in the future.
    /// </exception>
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

        var months = DateHelper.GetMonthsInYear(year);
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
            
            // If the statistics for the month exist, add them to the list, else create them
            if (statistics != null)
            {
                yearsStatistics.Add(statistics);
            }
            else
            {
               var monthStartAndEnd = DateHelper.GetMonthStartAndEndDate(month); 
               
               // Get the count of establishments and users created in the month
                var establishmentsCount = await _establishmentService.GetEstablishmentsCountCreatedInMonthAsync(monthStartAndEnd.StartDate, monthStartAndEnd.EndDate);
                var userCount = await _userService.GetUsersCountCreatedInMonthAsync(monthStartAndEnd.StartDate, monthStartAndEnd.EndDate);
                
                var newStatistic = new MonthlyStatistic
                {
                    Date = month,
                    UsersCreatedCount = userCount,
                    EstablishmentsCreatedCount = establishmentsCount
                };
                
                // Create the new statistics, and add them to the list
                await _monthlyStatisticRepository.CreateMonthlyStatisticAsync(newStatistic);
                yearsStatistics.Add(newStatistic);
            }
        }
        
        return yearsStatistics;
    }
}