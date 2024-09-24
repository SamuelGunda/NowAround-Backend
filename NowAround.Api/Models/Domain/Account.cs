using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

public class Account
{
    public int Id { get; set; }
    public int Auth0Id { get; set; }
    
    public required Role Role { get; set; }
    
    public virtual User? User { get; set; }
    public virtual Establishment? Establishment { get; set; }
}