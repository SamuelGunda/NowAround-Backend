using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

public class Establishment
{
    public int Id { get; set; }
    public required string Auth0Id { get; set; }
    
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public required string City { get; set; }
    public required string Address { get; set; }
    public string Website { get; set; } = string.Empty; //TODO: Change to socials
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required int PriceCategory { get; set; }
    public RequestStatus RequestStatus { get; set; }
    
    public virtual ICollection<EstablishmentCategory> EstablishmentCategories { get; set; } = new List<EstablishmentCategory>();
    public virtual ICollection<EstablishmentTag> EstablishmentTags { get; set; } = new List<EstablishmentTag>();
    
    
    public EstablishmentDto ToDto()
    {
        return new EstablishmentDto
        {
            Name = Name,
            Description = Description,
            City = City,
            Address = Address,
            Latitude = Latitude,
            Longitude = Longitude,
            PriceCategory = PriceCategory,
            RequestStatus = RequestStatus,
            EstablishmentCategories = EstablishmentCategories,
            EstablishmentTags = EstablishmentTags
        };
    }
}