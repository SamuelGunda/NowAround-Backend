using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

public class User
{
    public int Id { get; set; }
    public required string Auth0Id { get; set; }
    
    public required Role Role { get; set; }
    
    public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();

    public string ToString()
    {
        return $"Id: {Id}, Auth0Id: {Auth0Id}, Role: {Role}";
    }
}