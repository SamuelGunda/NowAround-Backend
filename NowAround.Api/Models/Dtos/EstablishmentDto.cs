using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Dtos;

public class EstablishmentDto
{
    public string? Auth0Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public PriceCategory? PriceCategory { get; set; }
    public RequestStatus? RequestStatus { get; set; }
    
    public virtual ICollection<EstablishmentCategory>? EstablishmentCategories { get; set; }
    public virtual ICollection<EstablishmentTag>? EstablishmentTags { get; set; }
    
    
}