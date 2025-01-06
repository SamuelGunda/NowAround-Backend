using System.Net;
using Microsoft.AspNetCore.Http;
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
    
    // UpdateUserPictureAsync Tests
    
    [Fact]
    public async Task UpdateUserPictureAsync_ShouldReturnCreated()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "User auth0|valid");

        var picture = new FormFile(new MemoryStream("Test picture"u8.ToArray()), 0, "Test picture".Length, "Picture",
            "test.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
        
        var content = new MultipartFormDataContent();
        
        var pictureContent = new StreamContent(picture.OpenReadStream());
        pictureContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(picture.ContentType);
        content.Add(pictureContent, "Picture", picture.FileName);
        
        // Act
        var response = await client.PutAsync("/api/user/profile-picture", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateUserPictureAsync_IfParameterIsNull_ShouldReturnBadRequest()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "User auth0|valid");
        
        // Act
        var response = await client.PutAsync("/api/user/profile-picture", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateUserPictureAsync_IfUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "User auth0|invalid");
        
        var picture = new FormFile(new MemoryStream("Test picture"u8.ToArray()), 0, "Test picture".Length, "Picture",
            "test.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
        
        var content = new MultipartFormDataContent();
        
        var pictureContent = new StreamContent(picture.OpenReadStream());
        pictureContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(picture.ContentType);
        content.Add(pictureContent, "Picture", picture.FileName);
        
        // Act
        var response = await client.PutAsync("/api/user/profile-picture", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}