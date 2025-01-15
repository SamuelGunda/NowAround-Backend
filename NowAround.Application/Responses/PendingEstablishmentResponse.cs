namespace NowAround.Application.Responses;

public sealed record PendingEstablishmentResponse
(
    string Auth0Id, 
    string Name, 
    string OwnerName
);