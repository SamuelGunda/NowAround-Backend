using System.Text;
using Newtonsoft.Json;
using NowAround.Api.Interfaces;
using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Services;

public class Auth0Service : IAuth0Service
{
    
    private readonly HttpClient _httpClient;
    private readonly string _domain;
    private readonly string _clientId;
    private readonly string _clientSecret;
    
    public Auth0Service(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _domain = configuration["Auth0:Domain"];
        _clientId = configuration["Auth0:ClientId"];
        _clientSecret = configuration["Auth0:ClientSecret"];
    }
    
    public async Task<string> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var requestBody = new
        {
            client_id = _clientId,
            email = registerUserDto.Email,
            password = registerUserDto.Password,
            connection = "Username-Password-Authentication",
            name = registerUserDto.FullName
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"https://{_domain}/dbconnections/signup", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create user: {errorResponse}");
        }
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseJson = JsonConvert.DeserializeObject<dynamic>(responseBody);
        string userId = responseJson._id;
        return userId;
    }

    public async Task<string> LoginUserAsync(LoginUserDto loginUserDto)
    {
        var requestBody = new
        {
            client_id = _clientId,
            client_secret = _clientSecret,
            grant_type = "password",
            username = loginUserDto.Email,
            password = loginUserDto.Password,
            connection = "Username-Password-Authentication"
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"https://{_domain}/oauth/token", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to login user: {errorResponse}");
        }
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseJson = JsonConvert.DeserializeObject<dynamic>(responseBody);
        Console.WriteLine(responseJson);
        string token = responseJson.access_token;
        return token;
    }
}