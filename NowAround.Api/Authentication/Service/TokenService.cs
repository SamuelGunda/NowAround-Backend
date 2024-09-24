using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Authentication.Models;

namespace NowAround.Api.Authentication.Service;


public class TokenService : ITokenService
{
    
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    
    private readonly string _domain;
    private readonly string _clientId;
    private readonly string _clientSecret;
    
    public TokenService(HttpClient httpClient, IConfiguration configuration, IMemoryCache memoryCache)
    {
        _httpClient = httpClient;
        _memoryCache = memoryCache;

        _domain = configuration["Auth0:Domain"] ?? throw new ArgumentNullException(configuration["Auth0:Domain"]);
        _clientId = configuration["Auth0:ClientId"] ?? throw new ArgumentNullException(configuration["Auth0:ClientId"]);
        _clientSecret = configuration["Auth0:ClientSecret"] ?? throw new ArgumentNullException(configuration["Auth0:ClientSecret"]);
    }

    public async Task<string> GetManagementAccessTokenAsync()
    {
        /*
         * Check if the token is already cached
         * If it is, return the cached token
         * If it is not, get a new token and cache it
         */
        
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
            grant_type = "client_credentials"
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    
        var response = await _httpClient.PostAsync($"https://{_domain}/oauth/token", content);
        
        /*
         * If the response is not successful, throw an exception
         */

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {errorResponse}");
            throw new Exception($"Failed to get access token: {response.StatusCode}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseJson = JsonConvert.DeserializeObject<ManagementTokenResponse>(responseBody) ?? throw new ArgumentNullException(responseBody);
        
        /*
         * Cached token expires in 86400 seconds (24 hours)
         * We set the cache expiration to the same time
         */
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(responseJson.ExpiresIn));
        
        _memoryCache.Set("managementAccessToken", responseJson.AccessToken, cacheEntryOptions);
        return responseJson.AccessToken;
    }
}