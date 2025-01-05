using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories;
using NowAround.Api.Services;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuth0Service> _auth0ServiceMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly Mock<IStorageService> _storageServiceMock;

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _auth0ServiceMock = new Mock<IAuth0Service>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _storageServiceMock = new Mock<IStorageService>();

        _userService = new UserService(
            _loggerMock.Object,
            _userRepositoryMock.Object,
            _auth0ServiceMock.Object,
            _storageServiceMock.Object);
    }

    // CreateUserAsync tests

    [Fact]
    public async Task CreateUserAsync_CreatesUserAndAssignsRole_ForValidAuth0Id()
    {
        // Arrange
        const string auth0Id = "auth0|valid";

        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(1);
        _auth0ServiceMock.Setup(s => s.AssignRoleAsync(auth0Id, "user")).Returns(Task.CompletedTask);

        // Act
        await _userService.CreateUserAsync(auth0Id, "Samuel Pačút");

        // Assert
        _userRepositoryMock.Verify(r => r.CreateAsync(It.Is<User>(u => u.Auth0Id == auth0Id)), Times.Once);
        _auth0ServiceMock.Verify(s => s.AssignRoleAsync(auth0Id, "user"), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_LogsErrorAndThrowsException_WhenUserCreationFails()
    {
        // Arrange
        const string auth0Id = "auth0|valid";

        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ThrowsAsync(new Exception("User creation failed"));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(auth0Id, "Samuel Pačút"));
        Assert.Equal("User creation failed", exception.Message);
    }

    [Fact]
    public async Task CreateUserAsync_LogsErrorAndThrowsException_WhenRoleAssignmentFails()
    {
        // Arrange
        const string auth0Id = "auth0|valid";

        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(1);
        _auth0ServiceMock.Setup(s => s.AssignRoleAsync(auth0Id, "user"))
            .ThrowsAsync(new Exception("Role assignment failed"));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(auth0Id, "Samuel Pačút"));
        Assert.Equal("Role assignment failed", exception.Message);
    }

    // GetUsersCountCreatedInMonthAsync tests

    [Fact]
    public async Task GetUsersCountCreatedInMonthAsync_ReturnsCorrectCount_ForValidDateRange()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 1, 31);
        const int expectedCount = 10;

        _userRepositoryMock.Setup(r => r.GetCountByCreatedAtBetweenDatesAsync(startDate, endDate))
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

        _userRepositoryMock.Setup(r => r.GetCountByCreatedAtBetweenDatesAsync(startDate, endDate))
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

        _userRepositoryMock.Setup(r => r.GetCountByCreatedAtBetweenDatesAsync(startDate, endDate))
            .ThrowsAsync(new ArgumentException("Invalid date range"));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.GetUsersCountCreatedInMonthAsync(startDate, endDate));
        Assert.Equal("Invalid date range", exception.Message);
    }

    // GetUserAsync tests

    [Fact]
    public async Task GetUserAsync_ReturnsUser_ForValidAuth0Id()
    {
        // Arrange
        const string auth0Id = "auth0|valid";
        var user = new User { Auth0Id = auth0Id, FullName = "Samuel Pačút" };

        _userRepositoryMock.Setup(r => r.GetByAuth0IdAsync(auth0Id)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserAsync(auth0Id);

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetUserAsync_ThrowsException_ForInvalidAuth0Id()
    {
        // Arrange
        const string auth0Id = "auth0|invalid";
        User? user = null;

        _userRepositoryMock.Setup(r => r.GetByAuth0IdAsync(auth0Id)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.GetUserAsync(auth0Id));
    }
}