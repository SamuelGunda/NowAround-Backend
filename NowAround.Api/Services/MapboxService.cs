
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NowAround.Api.Interfaces;

namespace NowAround.Api.Services;

public class MapboxService : IMapboxService
{

    private readonly HttpClient _httpClient;
    
    private readonly string _mapboxAccessToken;
    
    public MapboxService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _mapboxAccessToken = configuration["Mapbox:AccessToken"] ?? throw new ArgumentNullException(configuration["Mapbox:AccessToken"]);
    }
    
    /*
     * Get coordinates from address
     *
     * 1. Send request to Mapbox API
     * 2. Deserialize response
     * 3. Check if response is valid
     * 4. Get coordinates from response
     * 5. Return coordinates
     *
     * Example address: "Sládkovičova 1532, Žiar nad Hronom, Slovakia"
     */
    
    public async Task<(double lat, double lng)> GetCoordinatesFromAddressAsync(string address)
    {
        
        /*
         * The address is hard coded for the time being as the main focus is on the Slovakia
         * TODO: In future, the country should be passed as a parameter
         */
         
        address += ", Slovakia";
        
        var url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{Uri.EscapeDataString(address)}.json?access_token={_mapboxAccessToken}";
        var response = await _httpClient.GetStringAsync(url);
        
        var responseJson = JsonConvert.DeserializeObject<JObject>(response) ?? throw new ArgumentNullException(response);
        
        var coordinates = responseJson["features"]?[0]?["geometry"]?["coordinates"];
        
        if (coordinates == null || coordinates.Count() < 2)
        {
            throw new InvalidOperationException("Unable to retrieve valid coordinates from the API response.");
        }
        
        var lng = (double)(coordinates[0] ?? throw new InvalidOperationException());
        var lat = (double)(coordinates[1] ?? throw new InvalidOperationException());
        return (lat, lng);
    }
}