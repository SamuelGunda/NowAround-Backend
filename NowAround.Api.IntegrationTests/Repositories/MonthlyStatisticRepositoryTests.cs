﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Api.Database;
using NowAround.Api.IntegrationTests.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories;

namespace NowAround.Api.IntegrationTests.Repositories;

public class MonthlyStatisticRepositoryTests
{
    private readonly TestAppDbContext _context;
    private readonly MonthlyStatisticRepository _repository;
    private readonly SqliteConnection _connection;
    
    public MonthlyStatisticRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        
        _context = new TestAppDbContext(options);
        _repository = new MonthlyStatisticRepository(_context, Mock.Of<ILogger<MonthlyStatisticRepository>>());
        
        _context.Database.EnsureCreated();
    }
    
    // CreateMonthlyStatisticAsync tests
    
    [Fact]
    public async Task CreateMonthlyStatisticAsync_ValidMonthlyStatistic_CreatesMonthlyStatistic()
    {
        // Arrange
        var monthlyStatistic = new MonthlyStatistic
        {
            Date = "2022-01",
            UsersCreatedCount = 10,
            EstablishmentsCreatedCount = 5
        };
        
        // Act
        await _repository.CreateMonthlyStatisticAsync(monthlyStatistic);
        
        // Assert
        var result = await _context.MonthlyStatistics.FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.Equal("2022-01", result.Date);
        Assert.Equal(10, result.UsersCreatedCount);
        Assert.Equal(5, result.EstablishmentsCreatedCount);
    }
    
    [Fact]
    public async Task CreateMonthlyStatisticAsync_MonthlyStatisticIsNull_ThrowsException()
    {
        // Act
        async Task Act() => await _repository.CreateMonthlyStatisticAsync(null);
        
        // Assert
        await Assert.ThrowsAsync<Exception>(Act);
    }
    
    // GetMonthlyStatisticByDateAsync tests
    
    [Fact]
    public async Task GetMonthlyStatisticByDateAsync_MonthlyStatisticExists_ReturnsMonthlyStatistic()
    {
        // Arrange
        var monthlyStatistic = new MonthlyStatistic
        {
            Date = "2022-01",
            UsersCreatedCount = 10,
            EstablishmentsCreatedCount = 5
        };
        _context.MonthlyStatistics.Add(monthlyStatistic);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _repository.GetMonthlyStatisticByDateAsync("2022-01");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("2022-01", result.Date);
        Assert.Equal(10, result.UsersCreatedCount);
        Assert.Equal(5, result.EstablishmentsCreatedCount);
    }
    
    [Fact]
    public async Task GetMonthlyStatisticByDateAsync_MonthlyStatisticDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetMonthlyStatisticByDateAsync("2022-01");
        
        // Assert
        Assert.Null(result);
    }
}