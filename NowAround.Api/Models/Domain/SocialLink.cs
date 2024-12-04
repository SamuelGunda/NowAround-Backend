using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class SocialLink : BaseEntity
{
    public string Name { get; set; }
    public string Url { get; set; }
    public int EstablishmentId { get; set; }
    public Establishment Establishment { get; set; }
}