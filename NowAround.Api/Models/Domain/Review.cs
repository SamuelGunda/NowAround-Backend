using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class 
    Review : BaseEntity
{
    [Range(0, 5)]
    public required int Rating { get; set; }
    [MaxLength(512)]
    public string? Body { get; set; }
    
    public DateTime CreatedAt { get; set; }  = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    public required int UserId { get; set; }
    public User User { get; set; }
    public required int RatingCollectionId { get; set; }
    public RatingStatistic RatingStatistic { get; set; }
    
    public ReviewDto ToDto()
    {
        return new ReviewDto(Id, User?.Auth0Id, RatingStatistic?.Establishment?.Auth0Id, Rating, Body, CreatedAt);
    }
}