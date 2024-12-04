using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class MenuItem : BaseEntity
{
    public string Name { get; set; }
    public string PhotoUrl { get; set; }
    public string Description { get; set; }
    public string Price { get; set; }
    
    public int EstablishmentId { get; set; }
    public virtual Establishment Establishment { get; set; }
}