using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Application.Common.Exceptions;
using NowAround.Domain.Enum;
using NowAround.Domain.Models;
using NowAround.Infrastructure.Context;
using NowAround.Infrastructure.Repository.Specific;
using NowAround.IntegrationTests;

namespace NowAround.Infrastructure.UnitTests.Repository.Specific;

public class EstablishmentRepositoryTests
{

    private readonly TestAppDbContext _context;
    private readonly EstablishmentRepository _repository;

    public EstablishmentRepositoryTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
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
    
    // GetProfileByAuth0IdAsync tests
    
    [Fact]
    public async Task GetProfileByAuth0IdAsync_ShouldReturnEstablishment_WhenFound()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var establishment = new Establishment
        {
            Name = "Test Establishment",
            RequestStatus = RequestStatus.Accepted,
            City = "Test City",
            Address = "Test Address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable,
            Auth0Id = auth0Id
        };

        await _context.Set<Establishment>().AddAsync(establishment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProfileByAuth0IdAsync(auth0Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(auth0Id, result.Auth0Id);
        Assert.Equal("Test Establishment", result.Name);
    }

    [Fact]
    public async Task GetProfileByAuth0IdAsync_ShouldThrowEntityNotFoundException_WhenNotFound()
    {
        // Arrange
        const string nonExistentAuth0Id = "auth0|nonexistent";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _repository.GetProfileByAuth0IdAsync(nonExistentAuth0Id));

        Assert.Equal("The Establishment with Auth0 ID: auth0|nonexistent was not found", exception.Message);
    }
    
    [Fact]
    public async Task GetProfileByAuth0IdAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetProfileByAuth0IdAsync("auth0|123"));
    }
    
    // GetRangeWithFilterAsync tests
    
    [Fact]
    public async Task GetRangeWithFilterAsync_ShouldReturnEstablishments()
    {
        // Arrange
        var establishment1 = new Establishment
        {
            Name = "Test Name 1", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|123"
        };
        
        var establishment2 = new Establishment
        {
            Name = "Test Name 2", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|1234"
        };
        
        await _context.Set<Establishment>().AddRangeAsync(establishment1, establishment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRangeWithFilterAsync(q => q.Where(e => e.RequestStatus == RequestStatus.Accepted), 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Test Name 1", result[0].Name);
        Assert.Equal("Test Name 2", result[1].Name);
    }
    
    [Fact]
    public async Task GetRangeWithFilterAsync_WithNoPage_ShouldReturnEstablishments()
    {
        // Arrange
        var establishment1 = new Establishment
        {
            Name = "Test Name 1", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|123"
        };
        
        var establishment2 = new Establishment
        {
            Name = "Test Name 2", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|1234"
        };
        
        await _context.Set<Establishment>().AddRangeAsync(establishment1, establishment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRangeWithFilterAsync(q => q.Where(e => e.RequestStatus == RequestStatus.Accepted), 0);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Test Name 1", result[0].Name);
        Assert.Equal("Test Name 2", result[1].Name);
    }
        
    [Fact]
    public async Task GetRangeWithFilterAsync_IfNoEstablishments_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetRangeWithFilterAsync(q => q.Where(e => e.RequestStatus == RequestStatus.Accepted), 1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetRangeWithFilterAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetRangeWithFilterAsync(q => q.Where(e => e.RequestStatus == RequestStatus.Accepted), 1));
    }
}