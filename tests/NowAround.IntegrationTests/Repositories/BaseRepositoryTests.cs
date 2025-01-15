using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Application.Common.Exceptions;
using NowAround.Domain.Common;
using NowAround.Infrastructure.Context;
using NowAround.Infrastructure.Repository.Base;

namespace NowAround.IntegrationTests.Repositories;

public class TestEntity : BaseEntity
{
    public string Name { get; set; }
}

public class TestRepository : BaseRepository<TestEntity>
{
    public TestRepository(TestAppDbContext context, ILogger<TestEntity> logger)
        : base(context, logger)
    {
    }
}

public class BaseRepositoryTests
{
    private readonly TestAppDbContext _context;
    private readonly TestRepository _repository;
    private readonly SqliteConnection _connection;

    public BaseRepositoryTests ()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        
        _context = new TestAppDbContext(options);
        _repository = new TestRepository(_context, Mock.Of<ILogger<TestEntity>>());
        
        _context.Database.EnsureCreated();
    }
    
    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
    
    // CreateAsync tests
    
    [Fact]
    public async Task CreateAsync_ShouldAddEntity()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };

        // Act
        var resultId = await _repository.CreateAsync(testEntity);

        // Assert
        var entity = await _context.Set<TestEntity>().FindAsync(resultId);
        Assert.NotNull(entity);
        Assert.Equal("Test Name", entity.Name);
    }

    [Fact]
    public async Task CreateAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity); 
        await _context.SaveChangesAsync(); 

        var duplicateEntity = new TestEntity { Id = testEntity.Id, Name = "Duplicate Name" };
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(Act); 
        
        return; 
        async Task Act() => await _repository.CreateAsync(duplicateEntity);
    }
    
    // CheckIfExistsAsync tests
    
    [Fact]
    public async Task CheckIfExistsAsync_WhenEntityExists_ShouldReturnTrue()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.CheckIfExistsAsync(nameof(TestEntity.Name), "Test Name");

        // Assert
        Assert.True(exists);
    }
    
    [Fact]
    public async Task CheckIfExistsAsync_WhenEntityDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.CheckIfExistsAsync(nameof(TestEntity.Name), "Invalid Name");

        // Assert
        Assert.False(exists);
    }
    
    [Fact]
    public async Task CheckIfExistsAsync_WhenPropertyDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.CheckIfExistsAsync("InvalidPropertyName", "Test Name"));
    }

    // GetByIdAsync tests
    
    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ShouldReturnEntity()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        var entity = await _repository.GetByIdAsync(testEntity.Id);

        // Assert
        Assert.NotNull(entity);
        Assert.Equal("Test Name", entity.Name);
    }
    
    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        async Task Act() => await _repository.GetByIdAsync(testEntity.Id + 1);
        
        //Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(Act);
    }
    
    [Fact]
    public async Task GetByIdAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        await _context.DisposeAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(Act); 
        
        return; 
        async Task Act() => await _repository.GetByIdAsync(testEntity.Id);
    }
    
    // GetAsync tests
    
    [Fact]
    public async Task GetAsync_WhenEntityExists_ShouldReturnEntity()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(e => e.Name == "Test Name");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Name", result.Name);
    }

    [Fact]
    public async Task GetAsync_WhenEntityDoesNotExist_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _repository.GetAsync(e => e.Name == "Non-Existent Name"));
    }

    [Fact]
    public async Task GetAsync_IfExceptionOccurs_ShouldLogAndThrow()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<TestEntity>>();
        var repository = new TestRepository(_context, loggerMock.Object);

        await _context.DisposeAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => 
            repository.GetAsync(e => e.Name == "Test Name"));

        Assert.Equal("Failed to get TestEntity", exception.Message);
    }
    
    // GetAllAsync tests
    
    [Fact]
    public async Task GetAllAsync_WhenEntitiesExist_ShouldReturnEntities()
    {
        // Arrange
        var testEntity1 = new TestEntity { Name = "Test Name 1" };
        var testEntity2 = new TestEntity { Name = "Test Name 2" };
        await _context.Set<TestEntity>().AddRangeAsync(testEntity1, testEntity2);
        await _context.SaveChangesAsync();

        // Act
        var entities = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(entities);
        Assert.Equal(2, entities.Count());
        Assert.Contains(entities, e => e.Name == "Test Name 1");
        Assert.Contains(entities, e => e.Name == "Test Name 2");
    }
    
    [Fact]
    public async Task GetAllAsync_WhenNoEntitiesExist_ShouldReturnEmptyCollection()
    {
        // Act
        var entities = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(entities);
        Assert.Empty(entities);
    }
    
    [Fact]
    public async Task GetAllAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        await _context.DisposeAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(Act); 
        
        return; 
        async Task Act() => await _repository.GetAllAsync();
    }
    
    // UpdateAsync tests
    
    [Fact]
    public async Task UpdateAsync_WhenEntityExists_ShouldUpdateEntity()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        testEntity.Name = "Updated Name";
        await _repository.UpdateAsync(testEntity);

        // Assert
        var entity = await _context.Set<TestEntity>().FindAsync(testEntity.Id);
        Assert.NotNull(entity);
        Assert.Equal("Updated Name", entity.Name);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenEntityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var testEntity = new TestEntity { Id = 1, Name = "Test Name" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(Act); 
        
        return; 
        async Task Act() => await _repository.UpdateAsync(testEntity);
    }
    
    // DeleteAsync tests
    
    [Fact]
    public async Task DeleteAsync_WhenEntityExists_ShouldDeleteEntity()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(testEntity.Id);

        // Assert
        Assert.True(result);
        var entity = await _context.Set<TestEntity>().FindAsync(testEntity.Id);
        Assert.Null(entity);
    }
    
    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(testEntity.Id + 1);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task DeleteAsync_IfSomethingGoesWrong_ShouldThrowException()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test Name" };
        await _context.Set<TestEntity>().AddAsync(testEntity);
        await _context.SaveChangesAsync();

        await _context.DisposeAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(Act); 
        
        return; 
        async Task Act() => await _repository.DeleteAsync(testEntity.Id);
    }
    
    // DeleteRangeAsync tests
    
    [Fact]
    public async Task DeleteRangeAsync_WhenEntitiesExist_ShouldDeleteEntities()
    {
        // Arrange
        var testEntity1 = new TestEntity { Name = "Test Name 1" };
        var testEntity2 = new TestEntity { Name = "Test Name 2" };
        await _context.Set<TestEntity>().AddRangeAsync(testEntity1, testEntity2);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteRangeAsync(_context.Set<TestEntity>());
        await _context.SaveChangesAsync();
        
        // Assert
        var entities = await _context.Set<TestEntity>().ToListAsync();
        Assert.Empty(entities);
    }
    
    [Fact]
    public async Task DeleteRangeAsync_WhenNoEntitiesExist_ShouldNotThrowException()
    {
        // Act
        await _repository.DeleteRangeAsync(_context.Set<TestEntity>());
    }
}