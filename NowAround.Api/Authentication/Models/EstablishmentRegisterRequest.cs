using Newtonsoft.Json;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Authentication.Models;

public class EstablishmentRegisterRequest
{
    [JsonProperty("establishmentName")]
    public string Name { get; set; }
    
    [JsonProperty("establishmentPhoto")]
    public string? Photo { get; set; } //TODO: add default photo, if not provided
    
    [JsonProperty("establishmentAddress")]
    public string Adress { get; set; }
    
    [JsonProperty("establishmentCity")]
    public string City { get; set; }
    
    [JsonProperty("establishmentPrize")]
    public int PriceCategory { get; set; }
    
    [JsonProperty("establishmentCategory")]
    public ICollection<string> CategoryNames { get; set; } = new List<string>();
    
    [JsonProperty("establishmentTags")]
    public ICollection<string> TagNames { get; set; } = new List<string>();
    
    [JsonProperty("personalInfo")]
    public PersonalInfo PersonalInfo { get; set; }
}

public class PersonalInfo
{
    [JsonProperty("firstName")]
    public string? FName { get; set; }
    
    [JsonProperty("lastName")]
    public string? LName { get; set; }
    
    /*
    [JsonProperty("phoneNumber")]
    public string? PhoneNumber { get; set; }
    */
    
    [JsonProperty("email")]
    public string Email { get; set; }
}