using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class User : BaseAccountEntity
{
    [MaxLength(64)]
    public required string FullName { get; set; }
    public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Post> LikedPosts { get; set; } = new List<Post>();
}