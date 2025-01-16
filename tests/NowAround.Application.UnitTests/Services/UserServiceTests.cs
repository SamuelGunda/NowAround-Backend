using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Application.Common.Exceptions;
using NowAround.Application.Interfaces;
using NowAround.Application.Services;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;

namespace NowAround.Application.UnitTests.Services;

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
    
    // UpdateUserPictureAsync tests
    
    [Fact]
    public async Task UpdateUserPictureAsync_ThrowsArgumentException_WhenInvalidPictureContext()
    {
        // Arrange
        const string auth0Id = "auth0|valid";
        const string invalidPictureContext = "invalid-context";
        var pictureMock = new Mock<IFormFile>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _userService.UpdateUserPictureAsync(auth0Id, invalidPictureContext, pictureMock.Object));

        Assert.Equal("Invalid image context", exception.Message);
    }

    [Fact]
    public async Task UpdateUserPictureAsync_UpdatesProfilePictureUrl_WhenContextIsProfilePicture()
    {
        // Arrange
        const string auth0Id = "auth0|valid";
        const string pictureContext = "profile-picture";
        var pictureMock = new Mock<IFormFile>();
        const string pictureUrl = "http://storage.com/user/auth0|valid/profile-picture/xyz.jpg";

        var user = new User { Auth0Id = auth0Id, FullName = "Samuel Pačút" };

        _userRepositoryMock.Setup(r => r.GetAsync(u => u.Auth0Id == auth0Id, true))
            .ReturnsAsync(user);

        _storageServiceMock.Setup(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(pictureUrl);

        // Act
        var result = await _userService.UpdateUserPictureAsync(auth0Id, pictureContext, pictureMock.Object);

        // Assert
        Assert.Equal(pictureUrl, result);
        Assert.Equal(pictureUrl, user.ProfilePictureUrl);
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUserPictureAsync_UpdatesBackgroundPictureUrl_WhenContextIsBackgroundPicture()
    {
        // Arrange
        const string auth0Id = "auth0|valid";
        const string pictureContext = "background-picture";
        var pictureMock = new Mock<IFormFile>();
        const string pictureUrl = "http://storage.com/user/auth0|valid/background-picture/xyz.jpg";

        var user = new User { Auth0Id = auth0Id, FullName = "Samuel Pačút" };

        _userRepositoryMock.Setup(r => r.GetAsync(u => u.Auth0Id == auth0Id, true))
            .ReturnsAsync(user);

        _storageServiceMock.Setup(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(pictureUrl);

        // Act
        var result = await _userService.UpdateUserPictureAsync(auth0Id, pictureContext, pictureMock.Object);

        // Assert
        Assert.Equal(pictureUrl, result);
        Assert.Equal(pictureUrl, user.BackgroundPictureUrl);
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUserPictureAsync_LogsWarning_WhenInvalidContext()
    {
        // Arrange
        const string auth0Id = "auth0|valid";
        const string invalidPictureContext = "invalid-context";
        var pictureMock = new Mock<IFormFile>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _userService.UpdateUserPictureAsync(auth0Id, invalidPictureContext, pictureMock.Object));
    }

    [Fact]
    public async Task UpdateUserPictureAsync_CallsUpdateAsync_WhenValidContextAndPictureUploaded()
    {
        // Arrange
        const string auth0Id = "auth0|valid";
        const string pictureContext = "profile-picture";
        var pictureMock = new Mock<IFormFile>();
        const string pictureUrl = "http://storage.com/user/auth0|valid/profile-picture/xyz.jpg";

        var user = new User { Auth0Id = auth0Id, FullName = "Samuel Pačút", ProfilePictureUrl = "old-url" };

        _userRepositoryMock.Setup(r => r.GetAsync(u => u.Auth0Id == auth0Id, true))
            .ReturnsAsync(user);
        _storageServiceMock.Setup(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(pictureUrl);

        // Act
        await _userService.UpdateUserPictureAsync(auth0Id, pictureContext, pictureMock.Object);

        // Assert
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    // GetUserAsync tests

    [Fact]
    public async Task GetUserByAuth0IdAsync_ReturnsUser_ForValidAuth0Id()
    {
        // Arrange
        const string auth0Id = "auth0|valid";
        var user = new User { Auth0Id = auth0Id, FullName = "Samuel Pačút" };

        _userRepositoryMock.Setup(r => r.GetAsync(u => u.Auth0Id == auth0Id, false)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByAuth0IdAsync(auth0Id);

        // Assert
        Assert.Equal(user, result);
    }
    
    // DeleteUserAsync tests

    [Fact]
    public async Task DeleteUserAsync_DeletesUserSuccessfully_WhenValidAuth0Id()
    {
        // Arrange
        const string auth0Id = "auth0|valid";

        _auth0ServiceMock.Setup(a => a.DeleteAccountAsync(auth0Id)).Returns(Task.CompletedTask);
        _storageServiceMock.Setup(s => s.DeleteAsync("User", auth0Id, "")).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(r => r.DeleteByAuth0IdAsync(auth0Id)).ReturnsAsync(true);

        // Act
        await _userService.DeleteUserAsync(auth0Id);

        // Assert
        _auth0ServiceMock.Verify(a => a.DeleteAccountAsync(auth0Id), Times.Once);
        _storageServiceMock.Verify(s => s.DeleteAsync("User", auth0Id, ""), Times.Once);
        _userRepositoryMock.Verify(r => r.DeleteByAuth0IdAsync(auth0Id), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ThrowsEntityNotFoundException_WhenUserDeletionFails()
    {
        // Arrange
        const string auth0Id = "auth0|valid";

        _auth0ServiceMock.Setup(a => a.DeleteAccountAsync(auth0Id)).Returns(Task.CompletedTask);
        _storageServiceMock.Setup(s => s.DeleteAsync("User", auth0Id, "")).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(r => r.DeleteByAuth0IdAsync(auth0Id)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.DeleteUserAsync(auth0Id));
        Assert.Equal($"The User with Auth0 ID: {auth0Id} was not found", exception.Message);}

    [Fact]
    public async Task DeleteUserAsync_LogsErrorAndThrowsException_WhenAuth0DeleteFails()
    {
        // Arrange
        const string auth0Id = "auth0|valid";

        _auth0ServiceMock.Setup(a => a.DeleteAccountAsync(auth0Id)).ThrowsAsync(new Exception("Auth0 delete failed"));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _userService.DeleteUserAsync(auth0Id));
        Assert.Equal("Auth0 delete failed", exception.Message);
        _auth0ServiceMock.Verify(a => a.DeleteAccountAsync(auth0Id), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_LogsErrorAndThrowsException_WhenStorageDeleteFails()
    {
        // Arrange
        const string auth0Id = "auth0|valid";

        _auth0ServiceMock.Setup(a => a.DeleteAccountAsync(auth0Id)).Returns(Task.CompletedTask);
        _storageServiceMock.Setup(s => s.DeleteAsync("User", auth0Id, "")).ThrowsAsync(new Exception("Storage delete failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _userService.DeleteUserAsync(auth0Id));
        Assert.Equal("Storage delete failed", exception.Message);
        _storageServiceMock.Verify(s => s.DeleteAsync("User", auth0Id, ""), Times.Once);
    }
}