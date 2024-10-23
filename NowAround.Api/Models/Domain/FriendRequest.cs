using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

public class FriendRequest
{
    public int Id { get; set; }
    
    public int SenderId { get; set; }
    public virtual User Sender { get; set; }
    
    public int ReceiverId { get; set; }
    public virtual User Receiver { get; set; }
    
    public RequestStatus Status { get; set; }
}