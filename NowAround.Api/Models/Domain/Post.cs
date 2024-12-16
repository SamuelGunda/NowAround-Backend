using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class Post : BaseEntity
{
    public required string Headline { get; set; }
    public required string Body { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    public int EstablishmentId { get; set; }
    public virtual Establishment Establishment { get; set; }
}