using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories;
using NowAround.Api.UnitTests.Database;

namespace NowAround.Api.UnitTests.Repositories;

public class CategoryRepositoryTests
{
    private readonly TestAppDbContext _context;
    private readonly CategoryRepository _repository;
    private readonly SqliteConnection _connection;
    
    public CategoryRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        
        _context = new TestAppDbContext(options);
        _repository = new CategoryRepository(_context, Mock.Of<ILogger<Category>>());
        
        _context.Database.EnsureCreated();
    }
    
    // GetByNameWithTagsAsync tests
    
    [Fact]
    public async Task GetByNameWithTagsAsync_CategoryExists_ReturnsCategory()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test",
            Tags = new List<Tag>
            {
                new Tag { Name = "Tag1" },
                new Tag { Name = "Tag2" }
            }
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _repository.GetByNameWithTagsAsync("Test");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        Assert.Equal(2, result.Tags.Count);
    }
    
    [Fact]
    public async Task GetByNameWithTagsAsync_CategoryDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByNameWithTagsAsync("Test");
        
        // Assert
        Assert.Null(result);
    }
}