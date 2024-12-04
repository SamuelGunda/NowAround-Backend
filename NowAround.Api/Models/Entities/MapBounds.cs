namespace NowAround.Api.Models.Entities;

public class MapBounds
{
    public double NwLat { get; init; }
    public double NwLong { get; init; }
    public double SeLat { get; init; }
    public double SeLong { get; init; }

    public bool ValidateProperties()
    {
        if (this is { NwLat: 0, NwLong: 0, SeLat: 0, SeLong: 0 })
        {
            return false;
        }
        
        if (NwLat < -90 || NwLat > 90 || NwLong < -180 || NwLong > 180)
        {
            throw new ArgumentException("Coordinates are invalid");
        }
        
        if (SeLat < -90 || SeLat > 90 || SeLong < -180 || SeLong > 180)
        {
            throw new ArgumentException("Coordinates are invalid");
        }
        
        if (NwLat <= SeLat)
        {
            throw new ArgumentException("Northwest latitude must be greater than southeast latitude.");
        }
        
        if (NwLong >= SeLong)
        {
            throw new ArgumentException("Northwest longitude must be less than southeast longitude.");
        }

        return true;
    }
}