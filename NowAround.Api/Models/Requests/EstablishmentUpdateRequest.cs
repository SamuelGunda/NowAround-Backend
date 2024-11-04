namespace NowAround.Api.Models.Requests;

public class EstablishmentUpdateRequest
{
    public required string Auth0Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? PriceCategory { get; set; }
    public ICollection<string>? Category { get; set; }
    public ICollection<string>? Tags { get; set; }
}