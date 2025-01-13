using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Requests;

namespace NowAround.Api.Services.Interfaces;

public interface IReviewService
{
    Task<ReviewDto> CreateReviewAsync(string auth0Id, ReviewCreateRequest reviewCreateRequest);
    Task DeleteReviewAsync(string auth0Id, int reviewId);
}