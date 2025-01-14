using System.ComponentModel.DataAnnotations;
using NowAround.Domain.Common;
using NowAround.Domain.Enum;

namespace NowAround.Domain.Models;

public class Event : BaseEntity
{
    [MaxLength(64)]
    public required string Title { get; set; }
    [MaxLength(512)]
    public required string Body { get; set; }
    [MaxLength(16)]
    public string Price { get; set; }
    [MaxLength(8)]
    public string EventPriceCategory { get; set; }
    [MaxLength(32)]
    public required string City { get; set; }
    [MaxLength(64)]
    public required string Address { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    [MaxLength(16)]
    public string? MaxParticipants { get; set; }
    [MaxLength(256)]
    public string? PictureUrl { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public required EventCategory EventCategory { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<User> InterestedUsers { get; init; } = new List<User>();
    public int EstablishmentId { get; init; }
    public Establishment Establishment { get; init; }
}