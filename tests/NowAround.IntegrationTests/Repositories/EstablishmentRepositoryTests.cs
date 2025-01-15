using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Domain.Enum;
using NowAround.Domain.Models;
using NowAround.Infrastructure.Context;
using NowAround.Infrastructure.Repository.Specific;

namespace NowAround.IntegrationTests.Repositories;

public class EstablishmentRepositoryTests
{

    private readonly TestAppDbContext _context;
    private readonly EstablishmentRepository _repository;
    private readonly SqliteConnection _connection;
    
    public EstablishmentRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        
        _context = new TestAppDbContext(options);
        _repository = new EstablishmentRepository(_context, Mock.Of<ILogger<Establishment>>());
        
        _context.Database.EnsureCreated();
    }
    
    
    // GetAllWhereRegisterStatusPendingAsync tests
    
    [Fact]
    public async Task GetAllWhereRegisterStatusPendingAsync_ShouldReturnEstablishments()
    {
        // Arrange
        var establishment1 = new Establishment
        {
            Name = "Test Name 1", RequestStatus = RequestStatus.Pending,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|123"
        };
        
        var establishment2 = new Establishment
        {
            Name = "Test Name 2", RequestStatus = RequestStatus.Pending,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|1234"
        };
        
        await _context.Set<Establishment>().AddRangeAsync(establishment1, establishment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllWhereRegisterStatusPendingAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Test Name 1", result[0].Name);
        Assert.Equal("Test Name 2", result[1].Name);
    }
    
    [Fact]
    public async Task GetAllWhereRegisterStatusPendingAsync_IfNoEstablishments_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllWhereRegisterStatusPendingAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetAllWhereRegisterStatusPendingAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetAllWhereRegisterStatusPendingAsync());
    }
    
    // GetRangeWithFilterAsync tests
    
    
}