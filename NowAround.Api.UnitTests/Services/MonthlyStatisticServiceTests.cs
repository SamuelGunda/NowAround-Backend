using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Services;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.UnitTests.Services;

public class MonthlyStatisticServiceTests
{
    private readonly Mock<IMonthlyStatisticRepository> _monthlyStatisticRepository;
    private readonly Mock<IEstablishmentService> _establishmentService;
    private readonly Mock<IUserService> _userService;
    private readonly LoggerMock<MonthlyStatisticService> _logger;
    private readonly MonthlyStatisticService _service;

    public MonthlyStatisticServiceTests()
    {
        _monthlyStatisticRepository = new Mock<IMonthlyStatisticRepository>();
        _establishmentService = new Mock<IEstablishmentService>();
        _userService = new Mock<IUserService>();
        _logger = new LoggerMock<MonthlyStatisticService>();
        _service = new MonthlyStatisticService(
            _monthlyStatisticRepository.Object, 
            _establishmentService.Object, 
            _userService.Object, 
            _logger.Object);
    }

    // GetMonthlyStatisticByYearAsync tests
    
   [Fact]
    public async Task GetMonthlyStatisticByYearAsync_InvalidYear_ThrowsArgumentException()
    {
        // Arrange
        var invalidYear = "invalidYear";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetMonthlyStatisticByYearAsync(invalidYear));

        
        Assert.Equal("Year is not in the correct format", exception.Message);
        var logEntry = _logger.LoggedMessages
            .FirstOrDefault(log => log.LogLevel == LogLevel.Error && log.Message == "Year is not in the correct format");
    }

    [Fact]
    public async Task GetMonthlyStatisticByYearAsync_YearInFuture_ReturnsEmptyList()
    {
        // Arrange
        var futureYear = (DateTime.Now.Year + 1).ToString();

        // Act
        var result = await _service.GetMonthlyStatisticByYearAsync(futureYear);

        // Assert
        var logEntry = _logger.LoggedMessages
            .FirstOrDefault(log => log.LogLevel == LogLevel.Error && log.Message == "Year cannot be in the future");
    
        Assert.NotNull(logEntry);
    }

    [Fact]
    public async Task GetMonthlyStatisticByYearAsync_ExistingStatistics_ReturnsStatistics()
    {
        // Arrange
        var year = "2024";
        var months = new List<string> { $"{year}-01", $"{year}-02" };
        var statistics = new MonthlyStatistic { Date = $"{year}-01", UsersCreatedCount = 10, EstablishmentsCreatedCount = 5 };
        
        _monthlyStatisticRepository
            .Setup(repo => repo.GetMonthlyStatisticByDateAsync($"{year}-01"))
            .ReturnsAsync(statistics);
        _monthlyStatisticRepository
            .Setup(repo => repo.GetMonthlyStatisticByDateAsync($"{year}-02"))
            .ReturnsAsync((MonthlyStatistic)null);

        // Act
        var result = await _service.GetMonthlyStatisticByYearAsync(year);

        // Assert
        
        Assert.Equal(statistics.Date, result[0].Date);
        Assert.Equal(0, result[1].UsersCreatedCount);
        Assert.Equal(0, result[1].EstablishmentsCreatedCount);
    }

    [Fact]
    public async Task GetMonthlyStatisticByYearAsync_MonthInFuture_AddsZeroStatistic()
    {
        // Arrange
        var year = DateTime.Now.Year.ToString();

        
        _monthlyStatisticRepository
            .Setup(repo => repo.GetMonthlyStatisticByDateAsync(It.IsAny<string>()))
            .ReturnsAsync((MonthlyStatistic)null);

        // Act
        var result = await _service.GetMonthlyStatisticByYearAsync(year);

        // Assert
        Assert.Equal(0, result[0].UsersCreatedCount);
        Assert.Equal(0, result[0].EstablishmentsCreatedCount);
    }
}