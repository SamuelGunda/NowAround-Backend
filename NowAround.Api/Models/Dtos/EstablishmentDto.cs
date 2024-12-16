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
    
    public virtual BusinessHoursDto? BusinessHours { get; set; }
    public virtual ICollection<MenuDto>? Menus { get; set; }
    public virtual ICollection<SocialLinkDto>? SocialLinks { get; set; }
    public virtual ICollection<Category>? Categories { get; set; }
    public virtual ICollection<Tag>? Tags { get; set; }
}