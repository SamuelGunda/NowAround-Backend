namespace NowAround.Api.Models.Domain;

public class SocialLink
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string Url { get; set; }
    public int EstablishmentId { get; set; }
    public Establishment Establishment { get; set; }
}