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
    [MaxLength(32)]
    public required string City { get; set; }
    [MaxLength(64)]
    public required string Address { get; set; }
    [MaxLength(512)]
    public string Website { get; set; } = string.Empty;
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required PriceCategory PriceCategory { get; set; }
    public RequestStatus RequestStatus { get; set; }
    
    public int BusinessHoursId { get; set; }
    public virtual BusinessHours BusinessHours { get; set; }
    public int RatingCollectionId { get; set; }
    public virtual RatingStatistic RatingStatistic { get; set; }
    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
    public virtual ICollection<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
    public virtual ICollection<EstablishmentCategory> EstablishmentCategories { get; set; } = new List<EstablishmentCategory>();
    public virtual ICollection<EstablishmentTag> EstablishmentTags { get; set; } = new List<EstablishmentTag>(); 
    public virtual ICollection<EstablishmentCuisine> EstablishmentCuisines { get; set; } = new List<EstablishmentCuisine>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    
    /*public EstablishmentDto ToDto()
    {
        return new EstablishmentDto
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
            Categories = EstablishmentCategories.Select(ec => ec.Category).ToList(),
            Tags = EstablishmentTags.Select(et => et.Tag).ToList()
        };
    }*/

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

            CategoryNames = EstablishmentCategories.Select(ec => ec.Category.Name).ToList(),
            TagNames = EstablishmentTags.Select(et => et.Tag.Name).ToList()
        };
    }

    public EstablishmentResponse ToMarker()
    {
        return new EstablishmentResponse
        {
            Name = Name,
            Auth0Id = Auth0Id,
            Latitude = Latitude,
            Longitude = Longitude
        };
    }
}