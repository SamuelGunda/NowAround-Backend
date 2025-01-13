using Microsoft.EntityFrameworkCore;
using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Requests;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Services;

public class ReviewService : IReviewService
{
    private readonly ILogger<Review> _logger;
    private readonly IReviewRepository _reviewRepository;
    private readonly IEstablishmentService _establishmentService;
    private readonly IUserService _userService;
    
    public ReviewService(ILogger<Review> logger, IReviewRepository reviewRepository, IEstablishmentService establishmentService, IUserService userService)
    {
        _logger = logger;
        _reviewRepository = reviewRepository;
        _establishmentService = establishmentService;
        _userService = userService;
    }
    
    public async Task<ReviewDto> CreateReviewAsync(string auth0Id, ReviewCreateRequest reviewCreateRequest)
    {
        var establishment = await _establishmentService.GetEstablishmentByAuth0IdAsync(
            reviewCreateRequest.EstablishmentAuth0Id, 
            false, 
            e => e.Include(x => x.RatingStatistic));
        var user = await _userService.GetUserByAuth0IdAsync(auth0Id, false, u => u.Include(x => x.Reviews));

        if (user.Reviews.Any(r => r.RatingCollectionId == establishment.RatingStatistic.Id))
        {
            _logger.LogWarning("User {Auth0Id} has already reviewed establishment {EstablishmentAuth0Id}", auth0Id, reviewCreateRequest.EstablishmentAuth0Id);
            throw new EntityAlreadyExistsException("Review", "User", "User has already reviewed this establishment");        
        }
        
        var reviewEntity = new Review
        {
            Rating = reviewCreateRequest.Rating,
            Body = reviewCreateRequest.Body,
            RatingCollectionId = establishment.RatingStatistic.Id,
            UserId = user.Id
        };
        
        await _reviewRepository.CreateAsync(reviewEntity);
        
        await _establishmentService.UpdateRatingStatisticsAsync(establishment.RatingStatistic.Id, reviewCreateRequest.Rating);
        
        return reviewEntity.ToDto();
    }

    public async Task DeleteReviewAsync(string auth0Id, int reviewId)
    {
        var review = await _reviewRepository.GetAsync(
            r => r.Id == reviewId, 
            false, 
            r => r
                .Include(x => x.RatingStatistic)
                .Include(r => r.User));

        if (review.User.Auth0Id != auth0Id)
        {
            _logger.LogWarning("User {Auth0Id} tried to delete review {ReviewId} that does not belong to him", auth0Id, reviewId);
            throw new UnauthorizedAccessException("User is not allowed to delete this review");
        }
        
        await _reviewRepository.DeleteAsync(reviewId);
        
        await _establishmentService.UpdateRatingStatisticsAsync(review.RatingStatistic.Id, review.Rating, false);
    }
}