namespace NowAround.Api.Models.Domain;

public class EstablishmentCuisine
{
    public int EstablishmentId { get; init; }
    public Establishment Establishment { get; init; }
    public int CuisineId { get; init; }
    public Cuisine Cuisine { get; init; }
}