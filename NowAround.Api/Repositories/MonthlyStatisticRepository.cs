﻿using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories.Interfaces;

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