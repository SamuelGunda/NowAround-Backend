using System.ComponentModel.DataAnnotations;
using NowAround.Domain.Common;

namespace NowAround.Domain.Models;

public class Post : BaseEntity
{
    [MaxLength(64)]
    public required string Headline { get; set; }
    [MaxLength(512)]
    public required string Body { get; set; }
    [MaxLength(256)]
    public string? PictureUrl { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    public ICollection<User> Likes { get; init; } = new List<User>();
    public int EstablishmentId { get; init; }
    public Establishment Establishment { get; init; }
}