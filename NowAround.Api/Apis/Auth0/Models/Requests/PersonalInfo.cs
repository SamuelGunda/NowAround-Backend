namespace NowAround.Api.Apis.Auth0.Models.Requests;

public class PersonalInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
    /*
    TODO: Add phone number
    public string? PhoneNumber { get; set; }
    */

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