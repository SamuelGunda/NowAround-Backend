namespace NowAround.Api.Models.Domain;

public class EstablishmentTag
{
    public int EstablishmentId { get; init; }
    public Establishment Establishment { get; init; }
    public int TagId { get; init; }
    public Tag Tag { get; init; }
}