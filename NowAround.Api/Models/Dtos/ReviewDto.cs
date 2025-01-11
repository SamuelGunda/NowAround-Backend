namespace NowAround.Api.Models.Dtos;

public sealed record ReviewDto(
    int Id, 
    string ReviewerAuth0Id, 
    string EstablishmentAuth0Id, 
    int Rating, 
    string? Body, 
    DateTime CreatedAt
);