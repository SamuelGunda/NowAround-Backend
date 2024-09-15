using Microsoft.AspNetCore.Http.HttpResults;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

public class FriendRequest
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; }
    
    public int FriendId { get; set; }
    public virtual User Friend { get; set; }
    
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
}