using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

public class User
{
    public int Id { get; set; }
    public required string Auth0Id { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();
}