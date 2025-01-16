using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Logging;
using NowAround.Application.Requests;
using NowAround.Application.Services;
using NowAround.Domain.Enum;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;

namespace NowAround.Application.UnitTests.Services;

public class PostServiceTests
{
    private readonly Mock<ILogger<PostService>> _loggerMock;
    private readonly Mock<IEstablishmentService> _establishmentServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly PostService _postService;

    public PostServiceTests()
    {
        _loggerMock = new Mock<ILogger<PostService>>();
        _establishmentServiceMock = new Mock<IEstablishmentService>();
        _userServiceMock = new Mock<IUserService>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _storageServiceMock = new Mock<IStorageService>();

        _postService = new PostService(
            _loggerMock.Object,
            _establishmentServiceMock.Object,
            _userServiceMock.Object,
            _postRepositoryMock.Object,
            _storageServiceMock.Object
        );
    }
    
    // CreatePostAsync tests
    
    [Fact]
    public async Task CreatePostAsync_WhenNoPicture_CreatesPostSuccessfully()
    {
        // Arrange
        var postCreateRequest = new PostCreateUpdateRequest
        {
            Headline = "New Post",
            Body = "This is a new post body"
        };

        const string auth0Id = "auth0-id";
        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>()
        };

        _establishmentServiceMock
            .Setup(s => s.GetEstablishmentByAuth0IdAsync(It.IsAny<string>(), false))
            .ReturnsAsync(establishment);

        _postRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Post>()))
            .ReturnsAsync(1);

        // Act
        var result = await _postService.CreatePostAsync(postCreateRequest, auth0Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(postCreateRequest.Headline, result.Headline);
        Assert.Equal(postCreateRequest.Body, result.Body);
        _postRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Once);
        _storageServiceMock.Verify(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public async Task CreatePostAsync_WhenPicture_CreatesPostSuccessfully()
    {
        // Arrange
        var postCreateRequest = new PostCreateUpdateRequest
        {
            Headline = "New Post",
            Body = "This is a new post body",
            Picture = new FormFile(Stream.Null, 0, 0, "test-picture", "test-picture.jpg")
        };

        const string auth0Id = "auth0-id";
        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>()
        };

        _establishmentServiceMock
            .Setup(s => s.GetEstablishmentByAuth0IdAsync(It.IsAny<string>(), false))
            .ReturnsAsync(establishment);

        _postRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Post>()))
            .ReturnsAsync(1);

        _storageServiceMock
            .Setup(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("test-picture-url");

        // Act
        var result = await _postService.CreatePostAsync(postCreateRequest, auth0Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(postCreateRequest.Headline, result.Headline);
        Assert.Equal(postCreateRequest.Body, result.Body);
        Assert.Equal("test-picture-url", result.PictureUrl);
        _postRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Once);
        _storageServiceMock.Verify(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task CreatePostAsync_WhenEstablishmentNotFound_ShouldThrowException()
    {
        // Arrange
        var postCreateRequest = new PostCreateUpdateRequest
        {
            Headline = "New Post",
            Body = "This is a new post body"
        };

        const string auth0Id = "auth0-id";
        Establishment establishment = null;

        _establishmentServiceMock
            .Setup(s => s.GetEstablishmentByAuth0IdAsync(It.IsAny<string>(), false))
            .ReturnsAsync(establishment);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(Act);
        _postRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Never);
        _storageServiceMock.Verify(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        return;

        // Act
        async Task Act() => await _postService.CreatePostAsync(postCreateRequest, auth0Id);
    }
    
    // GetPostAsync tests

    [Fact]
    public async Task GetPostAsync_ReturnsPostSuccessfully()
    {
        // Arrange
        const int postId = 1;
        var postEntity = new Post
        {
            Id = postId,
            Headline = "test-headline",
            Body = "test-body",
            EstablishmentId = 1,
            Likes = new List<User>()
        };

        _postRepositoryMock
            .Setup(r => r.GetAsync(
                It.Is<Expression<Func<Post, bool>>>(e => e.Compile()(postEntity) == true),
                true, 
                It.IsAny<Func<IQueryable<Post>, IQueryable<Post>>>(), 
                It.IsAny<Func<IQueryable<Post>, IQueryable<Post>>>()
            ))
            .ReturnsAsync(postEntity);

        // Act
        var result = await _postService.GetPostAsync(postId, true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(postId, result.Id);
    }
    
    // ReactToPostAsync tests
    
    [Fact]
    public async Task ReactToPostAsync_WhenUserHasNotLiked_ShouldAddLikeSuccessfully()
    {
        // Arrange
        const int postId = 1;
        const string auth0Id = "auth0-id";
        var user = new User { Auth0Id = auth0Id, FullName = "Test User" };
        var postEntity = new Post
        {
            Id = postId,
            Headline = "test-headline",
            Body = "test-body",
            Likes = new List<User>()
        };

        _postRepositoryMock
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Post, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<Func<IQueryable<Post>, IQueryable<Post>>>()
            ))
            .ReturnsAsync(postEntity);
        _userServiceMock
            .Setup(s => s.GetUserByAuth0IdAsync(auth0Id, false))
            .ReturnsAsync(user);

        // Act
        await _postService.ReactToPostAsync(postId, auth0Id);

        // Assert
        Assert.Contains(postEntity.Likes, l => l.Auth0Id == auth0Id);
        _postRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Post>()), Times.Once);
    }
    
    [Fact]
    public async Task ReactToPostAsync_WhenUserHasLiked_ShouldRemoveLikeSuccessfully()
    {
        // Arrange
        const int postId = 1;
        const string auth0Id = "auth0-id";
        var user = new User { Auth0Id = auth0Id, FullName = "Test User" };
        var postEntity = new Post
        {
            Id = postId,
            Headline = "test-headline",
            Body = "test-body",
            Likes = new List<User> { user }
        };

        _postRepositoryMock
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Post, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<Func<IQueryable<Post>, IQueryable<Post>>>()
            ))
            .ReturnsAsync(postEntity);
        _userServiceMock
            .Setup(s => s.GetUserByAuth0IdAsync(auth0Id, false))
            .ReturnsAsync(user);

        // Act
        await _postService.ReactToPostAsync(postId, auth0Id);

        // Assert
        Assert.DoesNotContain(postEntity.Likes, l => l.Auth0Id == auth0Id);
        _postRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Post>()), Times.Once);
    }
    
    // DeletePostAsync tests
    
    [Fact]
    public async Task DeletePostAsync_DeletesPostSuccessfully()
    {
        // Arrange
        const int postId = 1;
        const string auth0Id = "auth0|123";

        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>()
        };

        var postEntity = new Post
        {
            Id = postId,
            Establishment = establishment,
            Headline = "test-headline",
            Body = "test-body",
            Likes = new List<User>()
        };

        _postRepositoryMock.Setup(pr => pr.GetAsync(
                It.Is<Expression<Func<Post, bool>>>(filter => filter.Compile()(postEntity)),
                false,
                It.IsAny<Func<IQueryable<Post>, IQueryable<Post>>>(),
                It.IsAny<Func<IQueryable<Post>, IQueryable<Post>>>()
            ))
            .ReturnsAsync(postEntity);

        // Act
        await _postService.DeletePostAsync(auth0Id, postId);

        // Assert
        _postRepositoryMock.Verify(pr => pr.GetAsync(
            It.IsAny<Expression<Func<Post, bool>>>(),
            false,
            It.IsAny<Func<IQueryable<Post>, IQueryable<Post>>>(),
            It.IsAny<Func<IQueryable<Post>, IQueryable<Post>>>()), Times.Once);

        _postRepositoryMock.Verify(pr => pr.DeleteAsync(postId), Times.Once);
    }
    
    [Fact]
    public async Task DeletePostAsync_WhenUserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        const int postId = 1;
        const string auth0Id = "auth0|123";

        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = "another-auth0-id",
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>()
        };

        var postEntity = new Post
        {
            Id = postId,
            Establishment = establishment,
            Headline = "test-headline",
            Body = "test-body",
            Likes = new List<User>()
        };

        _postRepositoryMock.Setup(pr => pr.GetAsync(
                It.Is<Expression<Func<Post, bool>>>(filter => filter.Compile()(postEntity)),
                false,
                It.IsAny<Func<IQueryable<Post>, IQueryable<Post>>>(),
                It.IsAny<Func<IQueryable<Post>, IQueryable<Post>>>()
            ))
            .ReturnsAsync(postEntity);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(Act);
        _postRepositoryMock.Verify(pr => pr.DeleteAsync(postId), Times.Never);
        return;

        // Act
        async Task Act() => await _postService.DeletePostAsync(auth0Id, postId);
    }
}
