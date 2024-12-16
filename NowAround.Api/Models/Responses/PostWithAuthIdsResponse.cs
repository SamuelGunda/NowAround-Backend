namespace NowAround.Api.Models.Dtos;

public sealed record PostWithAuthIdsResponse
(
    string EstablishmentAuth0Id,
    string Headline,
    string Body,
    string ImageUrl,
    ICollection<string> UserLikes,
    DateTime CreatedAt
);