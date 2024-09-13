namespace NowAround.Api.Models.Domain;

public class UserDetails
{
    public int Id { get; set; }
    
    public int AccountId { get; set; }
    public virtual Account Account { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
}