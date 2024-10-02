using System.Text;
using Newtonsoft.Json;
using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Authentication.Models;
using NowAround.Api.Authentication.Utilities;
using NowAround.Api.Database;
using NowAround.Api.Interfaces;
using NowAround.Api.Models.Domain;
using User = Auth0.ManagementApi.Models.User;

namespace NowAround.Api.Authentication.Service;

public class EstablishmentService : IEstablishmentService
{
    
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapboxService _mapboxService;
    
    private readonly string _domain;

    public EstablishmentService(HttpClient httpClient, AppDbContext context, ITokenService tokenService, IConfiguration configuration, IMapboxService mapboxService)
    {
        _httpClient = httpClient;
        _context = context;
        _tokenService = tokenService;
        _mapboxService = mapboxService;
        
        _domain = configuration["Auth0:Domain"] ?? throw new ArgumentNullException(configuration["Auth0:Domain"]);
    }
    
    public async Task<int> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest)
    {
        ArgumentNullException.ThrowIfNull(establishmentRequest);
        
        var personalInfo = establishmentRequest.PersonalInfo;
        var auth0Id = await RegisterEstablishmentOnAuth0(establishmentRequest.Name, personalInfo);
        
        var fullAddress = establishmentRequest.Adress + ", " + establishmentRequest.City;
        var coordinates = await _mapboxService.GetCoordinatesFromAddressAsync(fullAddress);
        
        var establishmentEntity = new Establishment()
        {
            Auth0Id = auth0Id,
            Name = establishmentRequest.Name,
            Latitude = coordinates.lat,
            Longitude = coordinates.lng,
            Address = establishmentRequest.Adress,
            City = establishmentRequest.City,
            PriceCategory = establishmentRequest.PriceCategory,
        };
        
        try 
        {
            await _context.Establishments.AddAsync(establishmentEntity);
            await _context.SaveChangesAsync();
            return establishmentEntity.Id;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create establishment", e);
        }
    }
    
    private async Task<string> RegisterEstablishmentOnAuth0(string establishmentName, PersonalInfo personalInfo)
    {
        ArgumentNullException.ThrowIfNull(personalInfo);

        var requestBody = new
        {
            email = personalInfo.Email,
            password = PasswordUtils.Generate(),
            name = establishmentName,
            given_name = personalInfo.FName,
            family_name = personalInfo.LName,
            connection = "Username-Password-Authentication"
        };
        
        var accessToken = await _tokenService.GetManagementAccessTokenAsync();
        
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_domain}/api/v2/users");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        request.Content = content;
        
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to create establishment:" + responseBody);
        }
        
        try
        {
            var user = JsonConvert.DeserializeObject<User>(responseBody);
            if (user == null)
            {
                throw new Exception("Failed to create establishment: User is null");
            }
            
            return user.UserId;
        }
        catch (JsonException ex)
        {
            throw new Exception("Failed to deserialize Auth0 response", ex);
        }
    }
}