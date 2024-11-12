using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class User : BaseEntity
{
    [MaxLength(32)]
    public required string Auth0Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();
}