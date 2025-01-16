using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NowAround.Application.Services;
using NowAround.Domain.Models;

namespace NowAround.WebApi.Controllers;

public class UserControllerTests
{
    
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _controller;
    private readonly ClaimsPrincipal _user;
    
    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _controller = new UserController(_mockUserService.Object);
        _user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "auth0Id123"),
        }));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = _user }
        };
    }
    
    private void SetUpAuth0Id(string auth0Id)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, auth0Id)
        };
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }
    
    // CreateUserAsync tests
    
    [Fact]
    public async Task CreateUserAsync_WhenUserIsCreatedSuccessfully_ReturnsCreatedResult()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        const string fullName = "John Doe";
    
        _mockUserService
            .Setup(service => service.CreateUserAsync(auth0Id, fullName))
            .Returns(Task.CompletedTask);

        var controller = new UserController(_mockUserService.Object);

        // Act
        var result = await controller.CreateUserAsync(auth0Id, fullName);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
    }
    
    // GetUserAsync tests

    /*[Fact]
    public async Task GetUserAsync_WhenUserExists_ReturnsOkResult()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var userDto = new User(auth0Id, "John Doe");
        
        
    }*/
    
    // UpdateUserPictureAsync tests
    
    [Fact]
    public async Task UpdateUserPictureAsync_ReturnsCreatedResult()
    {
        // Arrange
        const string auth0Id = "auth0Id123";
        const string pictureContext = "profile";
        var mockFile = new Mock<IFormFile>();
        const string pictureUrl = "http://example.com/picture.jpg";
        _mockUserService.Setup(service => service.UpdateUserPictureAsync(auth0Id, pictureContext, mockFile.Object))
            .ReturnsAsync(pictureUrl);
        SetUpAuth0Id(auth0Id);

        // Act
        var result = await _controller.UpdateUserPictureAsync(pictureContext, mockFile.Object);

        // Assert
        var actionResult = Assert.IsType<CreatedResult>(result);
    }
    
    // DeleteUserAsync tests

    [Fact]
    public async Task DeleteUserAsync_WhenUserIsDeleted_ReturnsNoContentResult()
    {
        // Arrange
        const string auth0Id = "auth0Id|123";
        _mockUserService.Setup(service => service.DeleteUserAsync(auth0Id)).Returns(Task.CompletedTask);
        SetUpAuth0Id(auth0Id);

        // Act
        var result = await _controller.DeleteUserAsync();

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
    }
}