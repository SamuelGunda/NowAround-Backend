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
        
        await _establishmentService.UpdateRatingStatisticsAsync(reviewCreateRequest.EstablishmentAuth0Id, reviewCreateRequest.Rating);
        
        return reviewEntity.ToDto();
    }
}