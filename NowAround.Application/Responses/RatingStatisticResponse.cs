using NowAround.Domain.Responses;

namespace NowAround.Application.Responses;

public sealed record RatingStatisticResponse
(
    int OneStar, 
    int TwoStar, 
    int ThreeStars, 
    int FourStars, 
    int FiveStars, 
    ICollection<ReviewWithAuthIdsResponse> Reviews
);