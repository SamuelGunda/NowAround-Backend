using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NowAround.Api.Apis.Mapbox.Interfaces;

namespace NowAround.Api.Apis.Mapbox.Services;

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
    /// Get coordinates from address asynchronously.
    /// Mapbox API is called to get coordinates from the address.
    /// The country is hard coded for the time being (Slovakia).
    /// TODO: In future, the country should be passed as a parameter
    /// </summary>
    
    public async Task<(double lat, double lng)> GetCoordinatesFromAddressAsync(string address)
    {
        address += ", Slovakia";
        
        // Call Mapbox API to get coordinates from address
        var url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{Uri.EscapeDataString(address)}.json?access_token={_mapboxAccessToken}";
        var response = await _httpClient.GetStringAsync(url);
        var responseJson = JsonConvert.DeserializeObject<JObject>(response);
        
        if (responseJson == null)
        {
            _logger.LogError("Unable to deserialize the API response.");
            throw new InvalidOperationException("Unable to deserialize the API response.");
        }
        
        // Get coordinates from the API response, if they are valid
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
}