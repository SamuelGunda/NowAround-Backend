using NowAround.Application.Dtos;
using NowAround.Application.Requests;

namespace NowAround.Application.Services;

public interface IReviewService
{
    Task<ReviewDto> CreateReviewAsync(string auth0Id, ReviewCreateRequest reviewCreateRequest);
    Task DeleteReviewAsync(string auth0Id, int reviewId);
}