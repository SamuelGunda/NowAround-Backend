namespace NowAround.Application.Interfaces;
public interface IMapboxService
{
    public Task<(double lat, double lng)> GetCoordinatesFromAddressAsync(string address, string postalCode, string city);
    
    public Task<(string address, string city)> GetAddressFromCoordinatesAsync(double lat, double lng);
}