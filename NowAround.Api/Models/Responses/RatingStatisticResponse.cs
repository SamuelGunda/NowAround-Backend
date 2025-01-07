using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Models.Responses;

public sealed record RatingStatisticResponse
(
    int OneStar, 
    int TwoStar, 
    int ThreeStars, 
    int FourStars, 
    int FiveStars, 
    ICollection<ReviewWithAuthIdsResponse> Reviews
);