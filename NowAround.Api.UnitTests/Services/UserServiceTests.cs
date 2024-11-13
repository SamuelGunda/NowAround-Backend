using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories;
using NowAround.Api.Services;

namespace NowAround.Api.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuth0Service> _auth0ServiceMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    
    private readonly UserService _userService;
    
    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _auth0ServiceMock = new Mock<IAuth0Service>();
        _loggerMock = new Mock<ILogger<UserService>>();
        
        _userService = new UserService(
            _loggerMock.Object, 
            _userRepositoryMock.Object, 
            _auth0ServiceMock.Object);
    }
    
    [Fact]
    public async Task CreateUserAsync_CreatesUserAndAssignsRole_ForValidAuth0Id()
    {
        // Arrange
        var auth0Id = "valid-auth0-id";
        var user = new User { Auth0Id = auth0Id };

        _userRepositoryMock.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(1);
        _auth0ServiceMock.Setup(s => s.AssignRoleToAccountAsync(auth0Id, "user")).Returns(Task.CompletedTask);

        // Act
        await _userService.CreateUserAsync(auth0Id);

        // Assert
        _userRepositoryMock.Verify(r => r.CreateUserAsync(It.Is<User>(u => u.Auth0Id == auth0Id)), Times.Once);
        _auth0ServiceMock.Verify(s => s.AssignRoleToAccountAsync(auth0Id, "user"), Times.Once);
    }
    
    [Fact]
    public async Task CreateUserAsync_LogsErrorAndThrowsException_WhenUserCreationFails()
    {
        // Arrange
        var auth0Id = "valid-auth0-id";
        var user = new User { Auth0Id = auth0Id };

        _userRepositoryMock.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ThrowsAsync(new Exception("User creation failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(auth0Id));
        Assert.Equal("Failed to create user", exception.Message);
    }

    [Fact]
    public async Task CreateUserAsync_LogsErrorAndThrowsException_WhenRoleAssignmentFails()
    {
        // Arrange
        var auth0Id = "valid-auth0-id";
        var user = new User { Auth0Id = auth0Id };

        _userRepositoryMock.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(1);
        _auth0ServiceMock.Setup(s => s.AssignRoleToAccountAsync(auth0Id, "user"))
            .ThrowsAsync(new Exception("Role assignment failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(auth0Id));
        Assert.Equal("Failed to create user", exception.Message);
    }
    
    /*[Fact]
    public async Task GetUsersCountCreatedInMonthAsync_ReturnsCorrectCount_ForValidDateRange()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 1, 31);
        var expectedCount = 10;

        _userRepositoryMock.Setup(r => r.GetUsersCountByCreatedAtBetweenDatesAsync(startDate, endDate))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _userService.GetUsersCountCreatedInMonthAsync(startDate, endDate);

        // Assert
        Assert.Equal(expectedCount, result);
    }

    [Fact]
    public async Task GetUsersCountCreatedInMonthAsync_ReturnsZero_ForNoUsersInDateRange()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 1, 31);
        var expectedCount = 0;

        _userRepositoryMock.Setup(r => r.GetUsersCountByCreatedAtBetweenDatesAsync(startDate, endDate))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _userService.GetUsersCountCreatedInMonthAsync(startDate, endDate);

        // Assert
        Assert.Equal(expectedCount, result);
    }

    [Fact]
    public async Task GetUsersCountCreatedInMonthAsync_ThrowsException_ForInvalidDateRange()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 31);
        var endDate = new DateTime(2023, 1, 1);

        _userRepositoryMock.Setup(r => r.GetUsersCountByCreatedAtBetweenDatesAsync(startDate, endDate))
            .ThrowsAsync(new ArgumentException("Invalid date range"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _userService.GetUsersCountCreatedInMonthAsync(startDate, endDate));
        Assert.Equal("Invalid date range", exception.Message);
    }*/
}