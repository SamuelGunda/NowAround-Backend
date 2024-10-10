using System.Text;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Newtonsoft.Json;
using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Authentication.Models;
using NowAround.Api.Authentication.Utilities;

namespace NowAround.Api.Authentication.Service;

public class AccountManagementService : IAccountManagementService
{
    
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    
    private readonly string _domain;
    
    public AccountManagementService(HttpClient httpClient, ITokenService tokenService, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        
        _domain = configuration["Auth0:Domain"] ?? throw new ArgumentNullException(configuration["Auth0:Domain"]);
    }
    
    public async Task<string> RegisterEstablishmentAccountAsync(string establishmentName, PersonalInfo personalInfo)
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
        
        /*
         * Get token to access Auth0 Management API
         */
        
        var accessToken = await _tokenService.GetManagementAccessTokenAsync();
        
        /*
         * Send request to create user
         */
        
        using var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_domain}/api/v2/users");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        request.Content = content;
        
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        /*
         * Check if request was successful
         */

        if (!response.IsSuccessStatusCode)
        {
            //TODO: Add custom exception for email already in use
            throw new HttpRequestException($"Failed to create establishment. Status Code: {response.StatusCode}, Response: {responseBody}");        
        }
        
        /*
         * Deserialize response and return user id
         * If deserialization fails, throw an exception
         * If user is null, throw an exception
         */
        
        try
        {
            var user = JsonConvert.DeserializeObject<User>(responseBody);
            if (user == null)
            {
                throw new InvalidOperationException("Failed to create establishment: User is null");
            }
            
            return user.UserId;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to deserialize Auth0 response", ex);
        }
    }

    public async Task<bool> DeleteAccountAsync(string auth0Id)
    {
        ArgumentNullException.ThrowIfNull(auth0Id);
        
        var accessToken = await _tokenService.GetManagementAccessTokenAsync();
        
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"https://{_domain}/api/v2/users/{Uri.EscapeDataString(auth0Id)}");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to delete account. Status Code: {response.StatusCode}, Response: {responseBody}");
        }

        return true;
    }
}