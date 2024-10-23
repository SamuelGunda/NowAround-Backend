namespace NowAround.Api.Models.Domain;

public class EstablishmentCategory
{
    public int EstablishmentId { get; init; }
    public Establishment Establishment { get; init; }
    public int CategoryId { get; init; }
    public Category Category { get; init; }
}