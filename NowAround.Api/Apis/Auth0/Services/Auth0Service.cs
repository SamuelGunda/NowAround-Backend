using System.Net;
using System.Text;
using Auth0.ManagementApi.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NowAround.Api.Apis.Auth0.Exceptions;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Apis.Auth0.Utilities;

namespace NowAround.Api.Apis.Auth0.Services;

public class Auth0Service : IAuth0Service
{
    
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly ILogger<Auth0Service> _logger;
    
    private readonly string _domain;
    private readonly string _establishmentRoleId;
    
    public Auth0Service(
        HttpClient httpClient, 
        ITokenService tokenService, 
        IConfiguration configuration, 
        ILogger<Auth0Service> logger)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _logger = logger;
        
        _domain = configuration["Auth0:Domain"] ?? throw new ArgumentNullException(configuration["Auth0:Domain"]);
        _establishmentRoleId = configuration["Auth0:Roles:Establishment"] ?? throw new ArgumentNullException(configuration["Auth0:Roles:Establishment"]);
    }
    
    /// <summary>
    /// Registers a new establishment account asynchronously.
    /// PersonalInfo is validated.
    /// Management API is called to create a new user.
    /// </summary>
    
    public async Task<string> RegisterEstablishmentAccountAsync(string establishmentName, PersonalInfo personalInfo)
    {
        personalInfo.ValidateProperties();
        
        var requestBody = new
        {
            email = personalInfo.Email,
            password = PasswordUtils.Generate(),
            name = establishmentName,
            given_name = personalInfo.FName,
            family_name = personalInfo.LName,
            connection = "Username-Password-Authentication"
        };
        
        // Get access token for Auth0 Management API
        var accessToken = await _tokenService.GetManagementAccessTokenAsync();
        
        // Send request to Auth0 API to create a new user
        using var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_domain}/api/v2/users");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        request.Content = content;
        
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        
        if (!response.IsSuccessStatusCode)
        {
            // If email is already in use, throw an exception
            if (response is {StatusCode: HttpStatusCode.Conflict})
            {
                _logger.LogWarning("Failed to create establishment. Email already in use. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
                throw new EmailAlreadyInUseException(personalInfo.Email);
            }
            
            // If request failed, throw an exception
            _logger.LogError("Failed to create establishment. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
            throw new HttpRequestException($"Failed to create establishment. Status Code: {response.StatusCode}, Response: {responseBody}");
        }
        
        // Deserialize response to get Auth0 user ID
        var user = JsonConvert.DeserializeObject<User>(responseBody) ?? throw new JsonException("Failed to deserialize Auth0 response");
        
        await AssignRoleToEstablishmentAsync(user.UserId, accessToken);
        
        return user.UserId;
    }

    public async Task DeleteAccountAsync(string auth0Id)
    {
        if (auth0Id.IsNullOrEmpty())
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        // Get access token for Auth0 Management API
        var accessToken = await _tokenService.GetManagementAccessTokenAsync();
        
        // Send request to Auth0 API to delete user
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"https://{_domain}/api/v2/users/{Uri.EscapeDataString(auth0Id)}");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine(responseBody);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to delete account. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
            throw new HttpRequestException($"Failed to delete account. Status Code: {response.StatusCode}, Response: {responseBody}");
        }
    }

    private async Task AssignRoleToEstablishmentAsync(string auth0Id, string accessToken)
    {
        var requestBody = new
        {
            roles = new[] { _establishmentRoleId }
        };
        
        using var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_domain}/api/v2/users/{auth0Id}/roles");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        request.Content = content;
        
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to assign role to establishment. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
            throw new HttpRequestException($"Failed to assign role to establishment. Status Code: {response.StatusCode}, Response: {responseBody}");
        }
    }
}