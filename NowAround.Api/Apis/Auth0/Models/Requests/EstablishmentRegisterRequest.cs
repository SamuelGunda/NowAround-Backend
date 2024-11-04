namespace NowAround.Api.Apis.Auth0.Models.Requests;

public class EstablishmentRegisterRequest
{
    public EstablishmentInfo EstablishmentInfo { get; set; }
    
    public PersonalInfo PersonalInfo { get; set; }
    
    public void ValidateProperties()
    {
        if (EstablishmentInfo == null)
        {
            throw new ArgumentNullException(nameof(EstablishmentInfo));
        }
        
        if (PersonalInfo == null)
        {
            throw new ArgumentNullException(nameof(PersonalInfo));
        }
        
        EstablishmentInfo.ValidateProperties();
        PersonalInfo.ValidateProperties();
    }
}