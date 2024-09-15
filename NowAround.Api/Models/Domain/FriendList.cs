namespace NowAround.Api.Models.Domain;

public class FriendList
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; }
    
    public ICollection<User> Friends { get; set; } = new List<User>();
}