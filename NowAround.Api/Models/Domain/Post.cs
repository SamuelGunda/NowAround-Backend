using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class Post : BaseEntity
{
    [MaxLength(64)]
    public required string Headline { get; set; }
    [MaxLength(512)]
    public required string Body { get; set; }
    [MaxLength(256)]
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    public int EstablishmentId { get; set; }
    public virtual Establishment Establishment { get; set; }
}