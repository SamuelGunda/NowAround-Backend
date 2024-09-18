using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

public class Account
{
    public int Id { get; set; }
    
    //TODO: Restructure the account model for auth0 integration
    
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public required Role Role { get; set; }
    
    public virtual User? User { get; set; }
    public virtual Establishment? Establishment { get; set; }
}