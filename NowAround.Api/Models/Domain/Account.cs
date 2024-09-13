using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

public class Account
{
    public int Id { get; set; }
    
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string Phone { get; set; }
    public string ProfilePicture { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Role Role { get; set; }
    
    public virtual UserDetails UserDetails { get; set; }
    public virtual EstDetails EstDetails { get; set; }
}