using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

public class Establishment
{
    public int Id { get; set; }
    public string Auth0Id { get; set; }
    
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string City { get; set; }
    public string Address { get; set; }
    public string Website { get; set; } = string.Empty; //TODO: Change to socials
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int PriceCategory { get; set; }
    public RequestStatus RequestStatus { get; set; }
    
    public virtual ICollection<EstablishmentCategory> EstablishmentCategories { get; } = new List<EstablishmentCategory>();
    public virtual ICollection<EstablishmentTag> EstablishmentTags { get; } = new List<EstablishmentTag>();
}