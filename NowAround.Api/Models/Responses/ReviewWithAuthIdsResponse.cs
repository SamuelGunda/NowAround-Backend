namespace NowAround.Api.Models.Dtos;

public sealed record ReviewWithAuthIdsResponse
(
    string UserAuth0Id, 
    string EstablishmentAuth0Id,
    string Fullname,
    string Body, 
    int Rating,
    DateTime CreatedAt
);