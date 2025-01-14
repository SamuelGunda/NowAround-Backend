namespace NowAround.Application.Interfaces;
public interface IMapboxService
{
    public Task<(double lat, double lng)> GetCoordinatesFromAddressAsync(string address, string postalCode, string city);
}