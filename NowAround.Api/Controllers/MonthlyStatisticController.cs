using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Interfaces;

namespace NowAround.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthlyStatisticController : ControllerBase
{
    private readonly ILogger<MonthlyStatisticController> _logger;
    private readonly IMonthlyStatisticService _monthlyStatisticService;
    
    public MonthlyStatisticController(
        IMonthlyStatisticService monthlyStatisticService, 
        ILogger<MonthlyStatisticController> logger)
    {
        _monthlyStatisticService = monthlyStatisticService;
        _logger = logger;
    }
    
    /// <summary>
    /// Retrieves the monthly statistics for a specific year,
    /// only an admin can access this endpoint.
    /// </summary>
    /// <param name="year"> The year to filter the monthly statistics by </param>
    /// <returns> The monthly statistics for the specified year.</returns>
    /// <response code="200"> Returns the monthly statistics for the specified year </response>
    /// <response code="400"> Returns an error message if the year is invalid </response>
    /// <response code="500"> Returns an error message if an exception occurs </response>
    /*
    [Authorize(Roles = "Admin")]
    */
    [HttpGet("{year}")]
    public async Task<IActionResult> GetMonthlyStatisticsByYearAsync(string year)
    {
        try
        {
            var statistics = await _monthlyStatisticService.GetMonthlyStatisticByYearAsync(year);

            if (statistics.Count == 0)
            {
                return BadRequest(new{ Title = "Invalid year", Detail = "Year cannot be in the future" });
            }
            
            return Ok(statistics);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting monthly statistics");
            throw;
        }
    }
}