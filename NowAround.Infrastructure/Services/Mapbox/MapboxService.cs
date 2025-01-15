using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NowAround.Application.Interfaces;

namespace NowAround.Infrastructure.Services.Mapbox;

public class MapboxService : IMapboxService
{

    private readonly HttpClient _httpClient;
    private readonly ILogger<MapboxService> _logger;
    
    private readonly string _mapboxAccessToken;
    
    public MapboxService(HttpClient httpClient, IConfiguration configuration, ILogger<MapboxService> logger)
    {
        _logger = logger;
        _httpClient = httpClient;
        _mapboxAccessToken = configuration["Mapbox:AccessToken"] ?? throw new ArgumentNullException(configuration["Mapbox:AccessToken"]);
    }
    
    /// <summary>
    /// Mapbox API is called to get coordinates from the address.
    /// The country is hard coded for the time being (Slovakia).
    /// TODO: In future, the country should be passed as a parameter
    /// </summary>
    
    public async Task<(double lat, double lng)> GetCoordinatesFromAddressAsync(string address, string postalCode, string city)
    {
        var country = Uri.EscapeDataString("Slovakia");
        address = Uri.EscapeDataString(address);
        city = Uri.EscapeDataString(city);
        postalCode = Uri.EscapeDataString(postalCode);
        
        // Call Mapbox API to get coordinates from address
        var url = $"https://api.mapbox.com/search/geocode/v6/forward?address_line1={address}&postcode={postalCode}&place={city}&country={country}&access_token={_mapboxAccessToken}";
        var response = await _httpClient.GetStringAsync(url);
        var responseJson = JsonConvert.DeserializeObject<JObject>(response);
        
        if (responseJson == null)
        {
            _logger.LogError("Unable to deserialize the API response.");
            throw new InvalidOperationException("Unable to deserialize the API response.");
        }
        
        var coordinates = responseJson["features"]?[0]?["geometry"]?["coordinates"];
        
        if (coordinates == null || coordinates.Count() < 2)
        {
            _logger.LogError("Unable to retrieve valid coordinates from the API response.");
            throw new InvalidOperationException("Unable to retrieve valid coordinates from the API response.");
        }
        
        var lng = (double)(coordinates[0] ?? throw new InvalidOperationException());
        var lat = (double)(coordinates[1] ?? throw new InvalidOperationException());
        
        return (lat, lng);
    }

    public async Task<(string address, string city)> GetAddressFromCoordinatesAsync(double lat, double lng)
    {
        var url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{lng},{lat}.json?access_token={_mapboxAccessToken}";

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var responseJson = JsonConvert.DeserializeObject<JObject>(response);
        
            if (responseJson == null)
            {
                _logger.LogError("Unable to deserialize the API response.");
                throw new InvalidOperationException("Unable to deserialize the API response.");
            }

            var placeName = responseJson["features"]?[0]?["place_name"]?.ToString();

            if (string.IsNullOrEmpty(placeName))
            {
                _logger.LogError("Unable to retrieve address from the API response.");
                throw new InvalidOperationException("Unable to retrieve address from the API response.");
            }
            
            var (address, city) = ParsePlaceName(placeName);
            return (address, city);
        }
        catch (InvalidOperationException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the address from Mapbox.");
            throw new InvalidOperationException("An error occurred while retrieving the address.", ex);
        }
    }
    
    private (string address, string city) ParsePlaceName(string placeName)
    {
        var parts = placeName.Split(',');

        if (parts.Length >= 2)
        {
            var address = parts[0].Trim();

            var city = parts[1].Trim();

            return (address, city);
        }

        _logger.LogError("The place name format is unexpected.");
        throw new InvalidOperationException("Unable to parse the place name.");
    }
}