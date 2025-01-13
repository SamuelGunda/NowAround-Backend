namespace NowAround.Api.Models.Entities;

public class MapBounds
{
    public double NwLat { get; init; }
    public double NwLong { get; init; }
    public double SeLat { get; init; }
    public double SeLong { get; init; }

    public bool ValidateProperties()
    {
        if (NwLat == 0 && NwLong == 0 && SeLat == 0 && SeLong == 0)
        {
            return false;
        }

        return NwLat > SeLat && NwLong < SeLong;
    }
}