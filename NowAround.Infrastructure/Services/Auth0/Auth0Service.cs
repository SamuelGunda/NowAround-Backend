﻿using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NowAround.Application.Common.Exceptions;
using NowAround.Application.Common.Helpers;
using NowAround.Application.Interfaces;
using NowAround.Application.Requests;
using NowAround.Application.Services;
using User = Auth0.ManagementApi.Models.User;

namespace NowAround.Infrastructure.Services.Auth0;

public class Auth0Service : IAuth0Service
{
    
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly IMailService _mailService;
    private readonly ILogger<Auth0Service> _logger;
    
    private readonly string _clientSecret;
    private readonly string _clientId;
    private readonly string _domain;
    private readonly string _establishmentRoleId;
    private readonly string _userRoleId;
    
    public Auth0Service(
        HttpClient httpClient, 
        ITokenService tokenService,
        IMailService mailService,
        IConfiguration configuration, 
        ILogger<Auth0Service> logger)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _mailService = mailService;
        _logger = logger;
        
        _clientSecret = configuration["Auth0:ClientSecret"] ?? throw new ArgumentNullException(configuration["Auth0:ClientSecret"]);
        _clientId = configuration["Auth0:ClientId"] ?? throw new ArgumentNullException(configuration["Auth0:ClientId"]);
        _domain = configuration["Auth0:Domain"] ?? throw new ArgumentNullException(configuration["Auth0:Domain"]);
        _establishmentRoleId = configuration["Auth0:Roles:Establishment"] ?? throw new ArgumentNullException(configuration["Auth0:Roles:Establishment"]);
        _userRoleId = configuration["Auth0:Roles:User"] ?? throw new ArgumentNullException(configuration["Auth0:Roles:User"]);
    }
    
    /// <summary>
    /// Registers a new establishment account by creating a new user in Auth0 and assigning a role to the user.
    /// Establishment goes into a pending state until the establishment is verified.
    /// </summary>
    /// <param name="establishmentOwnerInfo"> The personal information of the establishment owner </param>
    /// <returns> The Auth0 user ID of the newly created establishment account </returns>
    /// <exception cref="EmailAlreadyInUseException"> Thrown when the email is already in use </exception>
    /// <exception cref="HttpRequestException"> Thrown when the request to Auth0 API fails </exception>
    /// <exception cref="JsonException"> Thrown when the response from Auth0 API cannot be deserialized </exception>
    public async Task<string> RegisterEstablishmentAccountAsync(EstablishmentOwnerInfo establishmentOwnerInfo)
    {
        var requestBody = new
        {
            email = establishmentOwnerInfo.Email,
            password = PasswordUtils.Generate(),
            given_name = establishmentOwnerInfo.FirstName,
            family_name = establishmentOwnerInfo.LastName,
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
                throw new EmailAlreadyInUseException(establishmentOwnerInfo.Email);
            }
            
            // If request failed, throw an exception
            _logger.LogError("Failed to create establishment. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
            throw new HttpRequestException($"Failed to create establishment. Status Code: {response.StatusCode}, Response: {responseBody}");
        }
        
        // Deserialize response to get Auth0 user ID
        var user = JsonConvert.DeserializeObject<User>(responseBody) ?? throw new JsonException("Failed to deserialize Auth0 response");
        
        await AssignRoleAsync(user.UserId, "establishment");
        
        return user.UserId;
    }

    /// <summary>
    /// Gets the full name of the establishment owner by their Auth0 ID.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment owner </param>
    /// <returns> The full name of the establishment owner </returns>
    /// <exception cref="HttpRequestException"> Thrown when the request to Auth0 API fails </exception>
    /// <exception cref="JsonException"> Thrown when the response from Auth0 API cannot be deserialized </exception>
    public async Task<(string fullName, string email)> GetEstablishmentOwnerFullNameAndEmailAsync(string auth0Id)
    {
        if (string.IsNullOrEmpty(auth0Id))
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        var accessToken = await _tokenService.GetManagementAccessTokenAsync();
        
        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://{_domain}/api/v2/users/{Uri.EscapeDataString(auth0Id)}");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get establishment owner full name. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
            throw new HttpRequestException($"Failed to get establishment owner full name. Status Code: {response.StatusCode}, Response: {responseBody}");
        }
        
        var user = JsonConvert.DeserializeObject<User>(responseBody) 
                   ?? throw new JsonException("Failed to deserialize Auth0 response");
        
        return ($"{user.FirstName} {user.LastName}", user.Email);
    }

   public async Task ChangeAccountPasswordAsync(string auth0Id, string newPassword)
{
    /*var isOldPasswordValid = await VerifyOldPasswordAsync(auth0Id, oldPassword);
    if (!isOldPasswordValid)
    {
        _logger.LogWarning("Old password is incorrect for user {auth0Id}", auth0Id);
        throw new UnauthorizedAccessException("The old password is incorrect.");
    }*/

    var accessToken = await _tokenService.GetManagementAccessTokenAsync();

    var requestBody = new
    {
        password = newPassword,
        connection = "Username-Password-Authentication"
    };

    using var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
    using var request = new HttpRequestMessage(HttpMethod.Patch, $"https://{_domain}/api/v2/users/{auth0Id}");
    request.Headers.Add("Authorization", $"Bearer {accessToken}");
    request.Content = content;

    var response = await _httpClient.SendAsync(request);
    var responseBody = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        _logger.LogError("Failed to change account password. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
        throw new HttpRequestException($"Failed to change account password. Status Code: {response.StatusCode}, Response: {responseBody}");
    }

    _logger.LogInformation("Password successfully changed for user {auth0Id}", auth0Id);
}

    public async Task<bool> VerifyOldPasswordAsync(string auth0Id, string oldPassword)
    {
        var client = new HttpClient();

        var loginRequest = new
        {
            client_id = _clientId,
            client_secret = _clientSecret,
            grant_type = "password",
            username = auth0Id,
            password = oldPassword,
            scope = "openid"
        };

        var loginContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

        var loginResponse = await client.PostAsync($"https://{_domain}/oauth/token", loginContent);
        var loginResponseBody = await loginResponse.Content.ReadAsStringAsync();

        if (!loginResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("Login attempt failed for user {auth0Id}. Status Code: {StatusCode}, Response: {Response}",
                auth0Id, loginResponse.StatusCode, loginResponseBody);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Deletes an establishment account.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment to delete </param>
    /// <exception cref="ArgumentNullException"> Thrown when the provided Auth0 ID is null or empty </exception>
    /// <exception cref="HttpRequestException"> Thrown when the request to Auth0 API fails </exception>
    public async Task DeleteAccountAsync(string auth0Id)
    {
        if (string.IsNullOrEmpty(auth0Id))
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        var accessToken = await _tokenService.GetManagementAccessTokenAsync();
        
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"https://{_domain}/api/v2/users/{Uri.EscapeDataString(auth0Id)}");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to delete account. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
            throw new HttpRequestException($"Failed to delete account. Status Code: {response.StatusCode}, Response: {responseBody}");
        }
    }

    /// <summary>
    /// Assigns role to an account in Auth0.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the account </param>
    /// <param name="role"> The role to assign to the account </param>
    public async Task AssignRoleAsync(string auth0Id, string role)
    {
        if (string.IsNullOrEmpty(auth0Id))
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        var accessToken = await _tokenService.GetManagementAccessTokenAsync();
        
        role = role switch
        {
            "establishment" => _establishmentRoleId,
            "user" => _userRoleId,
            _ => throw new ArgumentException("Invalid role")
        };
        
        var requestBody = new
        {
            roles = new[] { role }
        };
        
        using var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_domain}/api/v2/users/{auth0Id}/roles");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        request.Content = content;
        
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to assign role. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
            throw new HttpRequestException($"Failed to assign role. Status Code: {response.StatusCode}, Response: {responseBody}");
        }
        
        await UpdateAppMetadataAsync(auth0Id, accessToken);
    }
    
    private async Task UpdateAppMetadataAsync(string auth0Id, string accessToken)
    {
        var requestBody = new
        {
            app_metadata = new
            {
                registeredInApi = true
            }
        };
        
        using var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"https://{_domain}/api/v2/users/{auth0Id}");
        request.Headers.Add("Authorization" , $"Bearer {accessToken}");
        request.Content = content;
        
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to update app metadata. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
            throw new HttpRequestException($"Failed to update app metadata. Status Code: {response.StatusCode}, Response: {responseBody}");
        }
    }
}