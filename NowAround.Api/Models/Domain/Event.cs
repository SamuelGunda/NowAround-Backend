using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

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
    public required double Duration { get; set; }
    public required EventCategory EventCategory { get; set; }
    [MaxLength(256)]
    public string? PictureUrl { get; set; }
    public required DateOnly DateOfEvent { get; set; }

    public ICollection<User> InterestedIn { get; init; } = new List<User>();
    public int EstablishmentId { get; init; }
    public Establishment Establishment { get; init; }
}*/