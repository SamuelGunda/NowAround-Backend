using Newtonsoft.Json;

namespace NowAround.Api.Apis.Auth0.Models.Requests;

public class EstablishmentRegisterRequest
{
    [JsonProperty("establishmentInfo")]
    public EstablishmentInfo EstablishmentInfo { get; set; }
    
    [JsonProperty("personalInfo")]
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

public class EstablishmentInfo
{
    [JsonProperty("establishmentName")]
    public string Name { get; set; }
    
    [JsonProperty("establishmentPhoto")]
    public string? Photo { get; set; } //TODO: add default photo, if not provided
    
    [JsonProperty("establishmentAddress")]
    public string Address { get; set; }
    
    [JsonProperty("establishmentCity")]
    public string City { get; set; }
    
    [JsonProperty("establishmentPrice")]
    public int PriceCategory { get; set; }
    
    [JsonProperty("establishmentCategory")]
    public ICollection<string> CategoryNames { get; set; }
    
    [JsonProperty("establishmentTags")]
    public ICollection<string>? TagNames { get; set; }
    
    public void ValidateProperties()
    {
        if (string.IsNullOrEmpty(Name) 
            || string.IsNullOrEmpty(Address) 
            || string.IsNullOrEmpty(City) 
            || PriceCategory < 0 || PriceCategory >= 3 
            || CategoryNames == null || CategoryNames.Count == 0)
        {
            throw new ArgumentNullException();
        }
    }
}

public class PersonalInfo
{
    [JsonProperty("firstName")]
    public string FName { get; set; }
    
    [JsonProperty("lastName")]
    public string LName { get; set; }
    
    /*
    [JsonProperty("phoneNumber")]
    public string? PhoneNumber { get; set; } //TODO: add phone number
    */
    
    [JsonProperty("email")]
    public string Email { get; set; }
    
    public void ValidateProperties()
    {
        if (string.IsNullOrEmpty(FName) 
            || string.IsNullOrEmpty(LName) 
            || string.IsNullOrEmpty(Email))
        {
            throw new ArgumentNullException();
        }
    }
}