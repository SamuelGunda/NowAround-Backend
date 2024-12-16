using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Responses;

namespace NowAround.Api.Models.Domain;

public class Establishment : BaseAccountEntity
{
    [MaxLength(32)]
    public required string Name { get; set; }
    [MaxLength(512)]
    public string Description { get; set; } = string.Empty;
    [MaxLength(512)]
    public string Website { get; set; } = string.Empty;
    [MaxLength(32)]
    public required string City { get; set; }
    [MaxLength(64)]
    public required string Address { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required PriceCategory PriceCategory { get; set; }
    public RequestStatus RequestStatus { get; set; }
    
    public BusinessHours BusinessHours { get; set; } = new();
    public RatingStatistic RatingStatistic { get; set; } = new();
    
    public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    public ICollection<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<EstablishmentTag> EstablishmentTags { get; set; } = new List<EstablishmentTag>(); 
    public ICollection<EstablishmentCuisine> EstablishmentCuisines { get; set; } = new List<EstablishmentCuisine>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public EstablishmentResponse ToDetailedResponse()
    {
        return new EstablishmentResponse
        {
            Auth0Id = Auth0Id,
            Name = Name,
            Description = Description,
            City = City,
            Address = Address,
            Latitude = Latitude,
            Longitude = Longitude,
            PriceCategory = PriceCategory,
            RequestStatus = RequestStatus,

            CategoryNames = Categories.Select(c => c.Name).ToList(),
            TagNames = EstablishmentTags.Select(et => et.Tag.Name).ToList()
        };
    }
}