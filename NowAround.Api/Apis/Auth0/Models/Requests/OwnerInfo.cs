namespace NowAround.Api.Apis.Auth0.Models.Requests;

public class OwnerInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public void ValidateProperties()
    {
        if (string.IsNullOrEmpty(FirstName)
            || string.IsNullOrEmpty(LastName)
            || string.IsNullOrEmpty(Email))
        {
            throw new ArgumentNullException();
        }
    }
}