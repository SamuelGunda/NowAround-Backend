using System.Text;
using Newtonsoft.Json;
using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Authentication.Models;
using NowAround.Api.Authentication.Utilities;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using User = Auth0.ManagementApi.Models.User;

namespace NowAround.Api.Authentication.Service;

public class EstablishmentService : IEstablishmentService
{
    
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly MapboxService _mapboxService;
    
    private readonly string _domain;

    public EstablishmentService(HttpClient httpClient, AppDbContext context, ITokenService tokenService, IConfiguration configuration, MapboxService mapboxService)
    {
        _httpClient = httpClient;
        _context = context;
        _tokenService = tokenService;
        _mapboxService = mapboxService;
        
        _domain = configuration["Auth0:Domain"] ?? throw new ArgumentNullException(configuration["Auth0:Domain"]);
    }
    
    public async Task<int> CreateEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest)
    {
        var requestBody = new
        {
            email = establishmentRequest.PersonalInfo.Email,
            password = PasswordUtils.Generate(),
            name = establishmentRequest.Name,
            given_name = establishmentRequest.PersonalInfo.FName,
            family_name = establishmentRequest.PersonalInfo.LName,
            connection = "Username-Password-Authentication",
        };
        
        var accessToken = _tokenService.GetManagementAccessTokenAsync().Result;
        
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_domain}/api/v2/users");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        request.Content = content;
        
        var response = _httpClient.SendAsync(request).Result;

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception("Failed to create establishment:" + errorResponse);
        }
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var user = JsonConvert.DeserializeObject<User>(responseBody);
        
        if (user == null)
        {
            throw new Exception("Failed to create establishment: User is null");
        }
        
        var fullAdress = establishmentRequest.Adress + ", " + establishmentRequest.City + ", Slovakia";
        
        var coordinates = await _mapboxService.GetCoordinatesFromAddress(fullAdress);
        
        var establishmentEntity = new Establishment()
        {
            Auth0Id = user.UserId,
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
            Console.WriteLine(e);
            throw new Exception("Failed to create establishment", e);
        }
    }
}