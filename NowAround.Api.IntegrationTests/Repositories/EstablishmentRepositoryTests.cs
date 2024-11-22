using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Api.Database;
using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Enum;
using NowAround.Api.Repositories;
using NowAround.Api.UnitTests.Database;

namespace NowAround.Api.UnitTests.Repositories;

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
    
    [Fact]
    public async Task GetRangeWithFilterAsync_ByName_ShouldReturnEstablishments()
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
        var result = await _repository.GetRangeWithFilterAsync("Test Name 1", null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Name 1", result[0].Name);
    }
    
    [Fact]
    public async Task GetRangeWithFilterAsync_ByNameAndPriceCategory_ShouldReturnEstablishments()
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
            PriceCategory = PriceCategory.Expensive, Auth0Id = "auth0|1234"
        };
        
        await _context.Set<Establishment>().AddRangeAsync(establishment1, establishment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRangeWithFilterAsync("Test Name 1", 0, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Name 1", result[0].Name);
    }
    
    [Fact]
    public async Task GetRangeWithFilterAsync_ByNamePriceCategoryAndCategory_ShouldReturnEstablishments()
    {
        var category = new Category { Name = "Test Category" };
        
        // Arrange
        var establishment1 = new Establishment
        {
            Name = "Test Name 1", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|123",
            EstablishmentCategories = new List<EstablishmentCategory>( new [] { new EstablishmentCategory { Category = category } })
        };
        
        var establishment2 = new Establishment
        {
            Name = "Test Name 2", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Expensive, Auth0Id = "auth0|1234"
        };
        
        await _context.Set<Establishment>().AddRangeAsync(establishment1, establishment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRangeWithFilterAsync("Test Name 1", 0, "Test Category", null);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Name 1", result[0].Name);
    }

    [Fact]
    public async Task GetRangeWithFilterAsync_ByNamePriceCategoryCategoryAndTag_ShouldReturnEstablishments()
    {
        var category = new Category { Name = "Test Category" };
        var tag = new Tag { Name = "Test Tag" };
        
        // Arrange
        var establishment1 = new Establishment
        {
            Name = "Test Name 1", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|123",
            EstablishmentCategories = new List<EstablishmentCategory>( new [] { new EstablishmentCategory { Category = category } }),
            EstablishmentTags = new List<EstablishmentTag>( new [] { new EstablishmentTag { Tag = tag } })
        };
        
        var establishment2 = new Establishment
        {
            Name = "Test Name 2", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Expensive, Auth0Id = "auth0|1234"
        };
        
        await _context.Set<Establishment>().AddRangeAsync(establishment1, establishment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRangeWithFilterAsync("Test Name 1", 0, "Test Category", ["Test Tag"]);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Name 1", result[0].Name);
    }
    
    
    [Fact]
    public async Task GetRangeWithFilterAsync_IfNoEstablishments_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetRangeWithFilterAsync("Test Name 1", 1, "Test Category", new List<string> { "Test Tag" });

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
        await Assert.ThrowsAsync<Exception>(() => _repository.GetRangeWithFilterAsync("Test Name 1", 1, "Test Category", new List<string> { "Test Tag" }));
    }
    
    // GetRangeWithFilterInAreaAsync tests
    
    [Fact]
    public async Task GetRangeWithFilterInAreaAsync_ShouldReturnEstablishments()
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
            City = "Test City", Address = "Test Address", Latitude = 2.0, Longitude = 2.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|1234"
        };
        
        await _context.Set<Establishment>().AddRangeAsync(establishment1, establishment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRangeWithFilterInAreaAsync(3.0, 0.0, 0.0, 3.0, null, null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Test Name 1", result[0].Name);
        Assert.Equal("Test Name 2", result[1].Name);
    }
    
    [Fact]
    public async Task GetRangeWithFilterInAreaAsync_IfNoEstablishments_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetRangeWithFilterInAreaAsync(3.0, 0.0, 0.0, 3.0, null, null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetRangeWithFilterInAreaAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetRangeWithFilterInAreaAsync(3.0, 0.0, 0.0, 3.0, null, null, null, null));
    }
    
    // UpdateAsync tests
    
    [Fact]
    public async Task UpdateAsync_WhenEntityExists_ShouldUpdateName()
    {
        // Arrange
        var establishment = new Establishment
        {
            Name = "Test Name", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|123"
        };
        
        var updatedEstablishment = new EstablishmentDto
        {
            Auth0Id = "auth0|123",
            Name = "Updated Name"
        };
        
        await _context.Set<Establishment>().AddAsync(establishment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.UpdateAsync(updatedEstablishment);

        // Assert
        var result = await _context.Set<Establishment>().FirstOrDefaultAsync(e => e.Auth0Id == "auth0|123");
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }
    
    [Fact]
    public async  Task UpdateAsync_WhenEntityExists_ShouldUpdateRequestStatus()
    {
        // Arrange
        var establishment = new Establishment
        {
            Name = "Test Name", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|123"
        };
        
        var updatedEstablishment = new EstablishmentDto
        {
            Auth0Id = "auth0|123",
            RequestStatus = RequestStatus.Rejected
        };
        
        await _context.Set<Establishment>().AddAsync(establishment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.UpdateAsync(updatedEstablishment);

        // Assert
        var result = await _context.Set<Establishment>().IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Auth0Id == "auth0|123");
        Assert.NotNull(result);
        Assert.Equal(RequestStatus.Rejected, result.RequestStatus);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_ShouldUpdateEntity()
    {
        
        // Arrange
        var category = new Category { Name = "Test Category" };
        var tag = new Tag { Name = "Test Tag" };
        var category2 = new Category { Name = "Test Category2" };
        var tag2 = new Tag { Name = "Test Tag2" };
                
        var establishment = new Establishment
        {
            Name = "Test Name", RequestStatus = RequestStatus.Accepted,
            City = "Test City", Address = "Test Address", Latitude = 1.0, Longitude = 1.0,
            PriceCategory = PriceCategory.Affordable, Auth0Id = "auth0|123",
            EstablishmentCategories = new List<EstablishmentCategory> { new EstablishmentCategory { Category = category } },
            EstablishmentTags = new List<EstablishmentTag>( new [] { new EstablishmentTag { Tag = tag } })
        };
        
        var updatedEstablishment = new EstablishmentDto
        {
            Auth0Id = "auth0|123",
            EstablishmentCategories = new List<EstablishmentCategory> { new() { Category = category }, new() { Category = category2 } },
            EstablishmentTags = new List<EstablishmentTag>([new EstablishmentTag { Tag = tag }, new EstablishmentTag { Tag = tag2 }
            ])
        };
        
        await _context.Set<Establishment>().AddAsync(establishment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.UpdateAsync(updatedEstablishment);
        
        // Assert
        var result = await _context.Set<Establishment>()
            .Include(e => e.EstablishmentCategories)
            .Include(e => e.EstablishmentTags)
            .FirstOrDefaultAsync(e => e.Auth0Id == "auth0|123");
        
        Assert.NotNull(result);
        Assert.Equal("Test Name", result.Name);
        Assert.Equal(RequestStatus.Accepted, result.RequestStatus);
        Assert.Equal("Test City", result.City);
        Assert.Equal("Test Address", result.Address);
        Assert.Equal(1.0, result.Latitude);
        Assert.Equal(1.0, result.Longitude);
        Assert.Equal(PriceCategory.Affordable, result.PriceCategory);
        Assert.Equal("auth0|123", result.Auth0Id);
        Assert.Equal(2, result.EstablishmentCategories.Count);
        Assert.Equal(2, result.EstablishmentTags.Count);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenEntityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var updatedEstablishment = new EstablishmentDto
        {
            Auth0Id = "auth0|123",
            Name = "Updated Name"
        };

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _repository.UpdateAsync(updatedEstablishment));
    }
    
    [Fact]
    public async Task UpdateAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.UpdateAsync(new EstablishmentDto()));
    }
}