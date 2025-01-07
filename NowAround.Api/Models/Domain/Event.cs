using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

//TODO: Implement Event, discuss with Rastik
public class Event; /*: BaseEntity
{
    [MaxLength(64)]
    public required string Title { get; set; }
    [MaxLength(512)]
    public required string Body { get; set; }
    public double? Price { get; set; }
    [MaxLength(64)]
    public required string Address { get; set; }
    public required string MaxParticipants { get; set; }
    [MaxLength(256)]
    public string? PictureUrl { get; set; }
    public required DateOnly Start { get; set; }
    public required DateOnly End { get; set; }
    public required EventCategory EventCategory { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<User> InterestedIn { get; init; } = new List<User>();
    public int EstablishmentId { get; init; }
    public Establishment Establishment { get; init; }
}*/