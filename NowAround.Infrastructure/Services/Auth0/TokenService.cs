﻿using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NowAround.Application.Interfaces;
using NowAround.Application.Responses;

namespace NowAround.Infrastructure.Services.Auth0;
public class TokenService : ITokenService
{
    
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<TokenService> _logger;
    
    private readonly string _domain;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _managementScopes;
    
    public TokenService(HttpClient httpClient, IConfiguration configuration, IMemoryCache memoryCache, ILogger<TokenService> logger)
    {
        _httpClient = httpClient;
        _memoryCache = memoryCache;
        _logger = logger;

        _domain = configuration["Auth0:Domain"] ?? throw new ArgumentNullException(configuration["Auth0:Domain"]);
        _clientId = configuration["Auth0:ClientId"] ?? throw new ArgumentNullException(configuration["Auth0:ClientId"]);
        _clientSecret = configuration["Auth0:ClientSecret"] ?? throw new ArgumentNullException(configuration["Auth0:ClientSecret"]);
        _managementScopes = configuration["Auth0:ManagementScopes"] ?? throw new ArgumentNullException(configuration["Auth0:ManagementScopes"]);
    }
    
    public async Task<string> GetManagementAccessTokenAsync()
    {
        var accessToken = _memoryCache.Get<string>("managementAccessToken");
        
        if (accessToken != null)
        {
            return accessToken;
        }
        
        var requestBody = new
        {
            client_id = _clientId,
            client_secret = _clientSecret,
            audience = $"https://{_domain}/api/v2/",
            grant_type = "client_credentials",
            scope =  _managementScopes
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"https://{_domain}/oauth/token", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to get access token: {StatusCode} {Error}", response.StatusCode, errorResponse);
            throw new Exception($"Failed to get access token: {response.StatusCode}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseJson = JsonConvert.DeserializeObject<ManagementTokenResponse>(responseBody) ?? throw new ArgumentNullException(responseBody);
        
        // Cache access token for future use, set expiration time to token expiration time
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(responseJson.ExpiresIn));
        
        _memoryCache.Set("managementAccessToken", responseJson.AccessToken, cacheEntryOptions);
        return responseJson.AccessToken;
    }
}