using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

public class User
{
    public int Id { get; set; }
    
    public required int AccountId { get; set; }
    public virtual required Account Account { get; set; }
    
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required Gender Gender { get; set; }
    public string ProfilePicture { get; set; }
    
    public virtual FriendList? FriendList { get; set; }
}