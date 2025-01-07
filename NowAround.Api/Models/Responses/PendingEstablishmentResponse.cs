namespace NowAround.Api.Models.Responses;

public sealed record PendingEstablishmentResponse
(
    string Auth0Id, 
    string Name, 
    string OwnerName
);