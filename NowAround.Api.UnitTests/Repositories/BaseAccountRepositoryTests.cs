using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Api.Database;
using NowAround.Api.Models.Entities;
using NowAround.Api.Repositories;
using NowAround.Api.UnitTests.Database;

namespace NowAround.Api.UnitTests.Repositories;

public class TestAccountEntity : BaseAccountEntity
{
    public string Name { get; set; }
}

public class TestAccountRepository : BaseAccountRepository<TestAccountEntity>
{
    public TestAccountRepository(TestAppDbContext context, ILogger<TestAccountEntity> logger)
        : base(context, logger)
    {
    }
}

public class BaseAccountRepositoryTests
{
    private readonly TestAppDbContext _context;
    private readonly TestAccountRepository _repository;
    private readonly Mock<ILogger<TestAccountEntity>> _mockLogger;
    
    public BaseAccountRepositoryTests ()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new TestAppDbContext(options);
        _repository = new TestAccountRepository(_context, Mock.Of<ILogger<TestAccountEntity>>());
        
        _context.Database.EnsureDeleted();
    }
    
    // GetByAuth0IdAsync tests
    
    [Fact]
    public async Task GetByAuth0IdAsync_ShouldReturnEntity()
    {
        // Arrange
        var testEntity = new TestAccountEntity { Name = "Test Name", Auth0Id = "auth0|123" };
        await _context.Set<TestAccountEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAuth0IdAsync("auth0|123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Name", result.Name);
    }
    
    [Fact]
    public async Task GetByAuth0IdAsync_IfEntityDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByAuth0IdAsync("auth0|123");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetByAuth0IdAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        var testEntity = new TestAccountEntity { Name = "Test Name", Auth0Id = "auth0|123" };
        await _context.Set<TestAccountEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();
        
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetByAuth0IdAsync("auth0|123"));
    }

    // GetCountByCreatedAtBetweenDatesAsync tests

    [Fact]
    public async Task GetCountByCreatedAtBetweenDatesAsync()
    {
        var testEntity1 = new TestAccountEntity { Name = "Test Name 1", Auth0Id = "auth0|123"};
        var testEntity2 = new TestAccountEntity { Name = "Test Name 2", Auth0Id = "auth0|124"};
        await _context.Set<TestAccountEntity>().AddRangeAsync(testEntity1, testEntity2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetCountByCreatedAtBetweenDatesAsync(DateTime.Now + TimeSpan.FromDays(-1), DateTime.Now + TimeSpan.FromDays(1));
        
        Assert.Equal(2, result);
    }
    
    [Fact]
    public async Task GetCountByCreatedAtBetweenDatesAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        var testEntity = new TestAccountEntity { Name = "Test Name", Auth0Id = "auth0|123" };
        await _context.Set<TestAccountEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();
        
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetCountByCreatedAtBetweenDatesAsync(DateTime.Now + TimeSpan.FromDays(-1), DateTime.Now + TimeSpan.FromDays(1)));
    }
    
    // DeleteByAuth0IdAsync tests
    
    [Fact]
    public async Task DeleteByAuth0IdAsync_WhenEntityExists_ShouldDeleteEntity()
    {
        // Arrange
        var testEntity = new TestAccountEntity { Name = "Test Name", Auth0Id = "auth0|123" };
        await _context.Set<TestAccountEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteByAuth0IdAsync("auth0|123");

        // Assert
        Assert.True(result);
        var entity = await _context.Set<TestAccountEntity>().FindAsync(testEntity.Id);
        Assert.Null(entity);
    }
    
    [Fact]
    public async Task DeleteByAuth0IdAsync_WhenEntityDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var testEntity = new TestAccountEntity { Name = "Test Name", Auth0Id = "auth0|123" };
        await _context.Set<TestAccountEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteByAuth0IdAsync("auth0|124");

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task DeleteByAuth0IdAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        var testEntity = new TestAccountEntity { Name = "Test Name", Auth0Id = "auth0|123" };
        await _context.Set<TestAccountEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();
        
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.DeleteByAuth0IdAsync("auth0|123"));
    }
}