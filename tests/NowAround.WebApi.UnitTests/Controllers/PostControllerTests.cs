using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NowAround.Application.Dtos;
using NowAround.Application.Requests;
using NowAround.Application.Services;
using NowAround.Domain.Models;

namespace NowAround.WebApi.Controllers;

public class PostControllerTests
{
    private readonly Mock<IPostService> _mockPostService;
    private readonly PostController _controller;
    private readonly ClaimsPrincipal _user;

    public PostControllerTests()
    {
        _mockPostService = new Mock<IPostService>();
        _controller = new PostController(_mockPostService.Object);
        _user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "auth0Id123"),
        }));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = _user }
        };
    }
    
    // CreatePostAsync tests

    [Fact]
    public async Task CreatePostAsync_ReturnsCreatedResult_WhenPostCreated()
    {
        // Arrange
        var postCreateRequest = new PostCreateUpdateRequest
        {
            Headline = "Sample Post",
            Body = "Sample Post Body"
        };

        var createdPostDto = new PostDto(
            1,
            "auth0Id123",
            "Sample Post",
            "Sample Post Body",
            null,
            DateTime.Now, 
            []
        );
        _mockPostService.Setup(service => service.CreatePostAsync(It.IsAny<PostCreateUpdateRequest>(), It.IsAny<string>()))
            .ReturnsAsync(createdPostDto);

        // Act
        var result = await _controller.CreatePostAsync(postCreateRequest);

        // Assert
        var actionResult = Assert.IsType<CreatedResult>(result);
        var returnValue = actionResult.Value as PostDto;
        Assert.NotNull(returnValue);
        _mockPostService.Verify(service => service.CreatePostAsync(It.IsAny<PostCreateUpdateRequest>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task CreatePostAsync_ThrowsArgumentException_WhenAuth0IdMissing()
    {
        // Arrange
        var invalidUser = new ClaimsPrincipal(new ClaimsIdentity());
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = invalidUser }
        };

        var postCreateRequest = new PostCreateUpdateRequest
        {
            Headline = "Sample Post",
            Body = "Sample Post Body"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.CreatePostAsync(postCreateRequest));
        Assert.Equal("Auth0Id not found", exception.Message);
    }
    
    // GetPostAsync tests

    [Fact]
    public async Task GetPostAsync_ReturnsOkResult_WhenPostFound()
    {
        // Arrange
        var postId = 1;
        var postDto = new PostDto(
            1,
            "auth0Id123",
            "Sample Post",
            "Sample Post Body",
            null,
            DateTime.Now, 
            []
        );

        _mockPostService.Setup(service => service.GetPostAsync(postId, false))
            .ReturnsAsync(new Post { Id = postId, Headline = "Sample Post", Body = "Sample Post Body" });

        // Act
        var result = await _controller.GetPostAsync(postId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = actionResult.Value as PostDto;
        Assert.NotNull(returnValue);
        Assert.Equal(postDto.Headline, returnValue.Headline);
        _mockPostService.Verify(service => service.GetPostAsync(postId, false), Times.Once);
    }
    
    // ReactToPostAsync tests

    [Fact]
    public async Task ReactToPostAsync_ReturnsOk_WhenPostReactedTo()
    {
        // Arrange
        var postId = 1;
        _mockPostService.Setup(service => service.ReactToPostAsync(postId, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ReactToPostAsync(postId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = actionResult.Value as dynamic;
        Assert.NotNull(returnValue);
        Assert.Equal("Reacted to post successfully", returnValue.message);
        _mockPostService.Verify(service => service.ReactToPostAsync(postId, It.IsAny<string>()), Times.Once);
    }
    
    // DeletePostAsync tests

    [Fact]
    public async Task DeletePostAsync_ReturnsOk_WhenPostDeleted()
    {
        // Arrange
        var postId = 1;
        _mockPostService.Setup(service => service.DeletePostAsync(It.IsAny<string>(), postId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeletePostAsync(postId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = actionResult.Value as dynamic;
        Assert.NotNull(returnValue);
        Assert.Equal("Post deleted successfully", returnValue.message);
        _mockPostService.Verify(service => service.DeletePostAsync(It.IsAny<string>(), postId), Times.Once);
    }
}