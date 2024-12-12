using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class Review : BaseEntity
{
    [Range(0, 5)]
    public required int Rating { get; set; }
    [MaxLength(512)]
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }  = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    public int UserId { get; set; }
    public required User User { get; set; }
    public int RatingCollectionId { get; set; }
    public required RatingStatistic RatingStatistic { get; set; }
}