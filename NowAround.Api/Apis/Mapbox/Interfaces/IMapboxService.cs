namespace NowAround.Api.Apis.Mapbox.Interfaces;

public interface IMapboxService
{
    public Task<(double lat, double lng)> GetCoordinatesFromAddressAsync(string address);
}