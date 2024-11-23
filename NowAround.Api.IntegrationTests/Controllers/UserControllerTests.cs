using System.Net;
using Moq;
using NowAround.Api.Apis.Auth0.Interfaces;

namespace NowAround.Api.IntegrationTests.Controllers;

public class UserControllerTests
{

    private readonly Mock<IAuth0Service> _auth0ServiceMock;
    
    public UserControllerTests()
    {
        _auth0ServiceMock = new Mock<IAuth0Service>();
    }
    
    // CreateUserAsync Tests

    [Fact]
    public async Task CreateUserAsync_ShouldReturnCreated()
    {
        // Arrange
        
        _auth0ServiceMock.Setup(x => x.AssignRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        
        var factory = new NowAroundWebApplicationFactory(_auth0ServiceMock);
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.PostAsync("/api/user?auth0Id=auth0|valid_register&fullName=Test Name", null);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateUserAsync_IfParameterIsNull_ShouldReturnBadRequest()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        
        // Act
        var response = await client.PostAsync("/api/user?fullName=Test Name", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateUserAsync_IfAuth0IdAlreadyExists_ShouldReturnInternalServerError()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        
        // Act
        var response = await client.PostAsync("/api/user?auth0Id=auth0|valid&fullName=Test Name", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    // GetUserAsync Tests
    
    [Fact]
    public async Task GetUserAsync_ShouldReturnOk()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/user?auth0Id=auth0|valid");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetUserAsync_IfParameterIsNull_ShouldReturnBadRequest()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/user");
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetUserAsync_IfUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/user?auth0Id=auth0|invalid");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}