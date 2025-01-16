using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Application.Common.Exceptions;
using NowAround.Application.Dtos;
using NowAround.Application.Requests;
using NowAround.Application.Services;
using NowAround.Domain.Enum;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;

namespace NowAround.Application.UnitTests.Services;

public class ReviewServiceTests
{
    private readonly Mock<ILogger<Review>> _mockLogger;
    private readonly Mock<IReviewRepository> _mockReviewRepository;
    private readonly Mock<IEstablishmentService> _mockEstablishmentService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly ReviewService _reviewService;

    public ReviewServiceTests()
    {
        _mockLogger = new Mock<ILogger<Review>>();
        _mockReviewRepository = new Mock<IReviewRepository>();
        _mockEstablishmentService = new Mock<IEstablishmentService>();
        _mockUserService = new Mock<IUserService>();
        _reviewService = new ReviewService(
            _mockLogger.Object,
            _mockReviewRepository.Object,
            _mockEstablishmentService.Object,
            _mockUserService.Object
        );
    }
    
    // CreateReviewAsync tests
    
    [Fact]
    public async Task CreateReviewAsync_WhenUserHasAlreadyReviewed_ShouldThrowEntityAlreadyExistsException()
    {
        // Arrange
        var reviewCreateRequest = new ReviewCreateRequest
        {
            EstablishmentAuth0Id = "auth0Id|123",
            Rating = 5,
            Body = "Great place!"
        };

        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = "auth0Id|123",
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Accepted,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            RatingStatistic = new RatingStatistic
            {
                Id = 1,
                EstablishmentId = 1,
            }
        };

        var user = new User
        {
            Id = 1,
            Auth0Id = "user123",
            FullName = "test-fullname",
            Reviews = new List<Review>
            {
                new() { RatingCollectionId = 1, Rating = 5, UserId = 1 }
            }
        };

        _mockEstablishmentService
            .Setup(service => service.GetEstablishmentByAuth0IdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>>()))
            .ReturnsAsync(establishment);

        _mockUserService
            .Setup(service => service.GetUserByAuth0IdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _reviewService.CreateReviewAsync("user123", reviewCreateRequest));
    }

    [Fact]
    public async Task CreateReviewAsync_WhenValid_ShouldCreateReviewSuccessfully()
    {
        // Arrange
        const string auth0Id = "auth0Id|123";
        
        var reviewCreateRequest = new ReviewCreateRequest
        {
            EstablishmentAuth0Id = "auth0Id|123",
            Rating = 5,
            Body = "Great place!"
        };

        var establishment = new Establishment
        {
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Accepted,
            Categories = new List<Category>(),
            Tags = new List<Tag>()
        };

        var user = new User
        {
            Id = 1,
            Auth0Id = "auth0Id|456",
            FullName = "test-fullname",
            Reviews = new List<Review>()
        };

        var reviewDto = new ReviewDto(
            1,
            "auth0Id|456",
            auth0Id,
            5,
            "Great place!",
            DateTime.Now
            );

        _mockEstablishmentService
            .Setup(service => service.GetEstablishmentByAuth0IdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>>()))
            .ReturnsAsync(establishment);

        _mockUserService
            .Setup(service => service.GetUserByAuth0IdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
            .ReturnsAsync(user);

        _mockReviewRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Review>()))
            .ReturnsAsync(1);

        _mockEstablishmentService
            .Setup(service => service.UpdateRatingStatisticsAsync(It.IsAny<int>(), It.IsAny<int>(),true))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _reviewService.CreateReviewAsync("user123", reviewCreateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reviewDto.Rating, result.Rating);
        Assert.Equal(reviewDto.Body, result.Body);
        _mockReviewRepository.Verify(repo => repo.CreateAsync(It.IsAny<Review>()), Times.Once);
    }

    [Fact]
    public async Task DeleteReviewAsync_WhenUserDoesNotOwnReview_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        const int reviewId = 1;
        var review = new Review
        {
            Id = reviewId,
            UserId = 2,
            User = new User
            {
                Id = 2,
                Auth0Id = "Test",
                FullName = "Test Name"
            },
            RatingCollectionId = 1,
            Rating = 4,
            Body = "Great place!",
            CreatedAt = DateTime.Now
        };
        
        _mockReviewRepository
            .Setup(repo => repo.GetAsync(
                It.Is<Expression<Func<Review, bool>>>(e => e.Compile().Invoke(new Review { Id = reviewId, UserId = 2, RatingCollectionId = 1, Rating = 4, Body = "Great place!", CreatedAt = DateTime.Now })), 
                false, 
                It.IsAny<Func<IQueryable<Review>, IQueryable<Review>>>()))
            .ReturnsAsync(review);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _reviewService.DeleteReviewAsync("user123", reviewId));
        Assert.Equal("User is not allowed to delete this review", exception.Message);
    }

    [Fact]
    public async Task DeleteReviewAsync_WhenValid_ShouldDeleteReviewSuccessfully()
    {
        const int reviewId = 1;
        var review = new Review
        {
            Id = reviewId,
            UserId = 1,
            RatingCollectionId = 1,
            User = new User
            {
                Id = 1,
                Auth0Id = "user123",
                FullName = "test-fullname"
            },
            RatingStatistic = new RatingStatistic
            {
                Id = 1,
                EstablishmentId = 1
            },
            Rating = 4,
            Body = "Great place!",
            CreatedAt = DateTime.Now
        };

        _mockReviewRepository
            .Setup(repo => repo.GetAsync(
                It.Is<Expression<Func<Review, bool>>>(e => e.Compile().Invoke(new Review { Id = reviewId, UserId = 1, RatingCollectionId = 1, Rating = 4, Body = "Great place!", CreatedAt = DateTime.Now })), 
                false, 
                It.IsAny<Func<IQueryable<Review>, IQueryable<Review>>>()))
            .ReturnsAsync(review);

        _mockReviewRepository
            .Setup(repo => repo.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _mockEstablishmentService
            .Setup(service => service.UpdateRatingStatisticsAsync(It.IsAny<int>(), It.IsAny<int>(), false))
            .Returns(Task.CompletedTask);

        // Act
        await _reviewService.DeleteReviewAsync("user123", reviewId);

        // Assert
        _mockReviewRepository.Verify(repo => repo.DeleteAsync(reviewId), Times.Once);
        _mockEstablishmentService.Verify(service => service.UpdateRatingStatisticsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
    }
}