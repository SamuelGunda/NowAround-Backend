using NowAround.Application.Dtos;
using NowAround.Domain.Models;

namespace NowAround.Application.Mapping;

public static class ReviewMapper
{
    public static ReviewDto ToDto(this Review reviewEntity)
    {
        ArgumentNullException.ThrowIfNull(reviewEntity, nameof(reviewEntity));

        return new ReviewDto(
            Id: reviewEntity.Id,
            ReviewerAuth0Id: reviewEntity.User?.Auth0Id ?? null,
            EstablishmentAuth0Id: reviewEntity.RatingStatistic?.Establishment?.Auth0Id ?? null,
            Rating: reviewEntity.Rating,
            Body: reviewEntity.Body,
            CreatedAt: reviewEntity.CreatedAt
        );
    }
}