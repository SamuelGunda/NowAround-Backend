namespace NowAround.Api.Models.Entities;

public class MapBounds
{
    public double NwLat { get; init; }
    public double NwLong { get; init; }
    public double SeLat { get; init; }
    public double SeLong { get; init; }
    
    public void ValidateProperties()
    {
        if (NwLat < -90 || NwLat > 90 || NwLong < -180 || NwLong > 180)
        {
            throw new ArgumentException("Invalid coordinates");
        }
        if (SeLat < -90 || SeLat > 90 || SeLong < -180 || SeLong > 180)
        {
            throw new ArgumentException("Invalid coordinates");
        }
    }
}