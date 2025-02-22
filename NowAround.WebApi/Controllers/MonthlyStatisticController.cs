﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NowAround.Application.Services;

namespace NowAround.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthlyStatisticController(IMonthlyStatisticService monthlyStatisticService) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("{year}")]
    public async Task<IActionResult> GetMonthlyStatisticsByYearAsync(string year)
    {
        var statistics = await monthlyStatisticService.GetMonthlyStatisticByYearAsync(year);

        if (statistics.Count == 0)
        {
            return BadRequest(new{ Title = "Invalid year", Detail = "Year cannot be in the future" });
        }
        
        return Ok(statistics);
    }
}