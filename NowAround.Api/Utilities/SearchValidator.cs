using NowAround.Api.Exceptions;

namespace NowAround.Api.Utilities;

public static class SearchValidator
{
    public static void ValidateFilterValues(string? name, string? categoryName, List<string>? tagNames, bool required)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(categoryName) && (tagNames == null || tagNames.Count == 0) && required)
        {
            throw new ArgumentException("At least one filter value must be provided");
        }
        
        if (!string.IsNullOrEmpty(name) && name.Length < 3)
        {
            throw new ArgumentException("Name is too short");
        }
    }
    
    public static void ValidateMapBounds(double nwLat, double nwLong, double seLat, double seLong)
    {
        if (nwLat < -90 || nwLat > 90 || nwLong < -180 || nwLong > 180)
        {
            throw new InvalidLocationException("Invalid coordinates");
        }
        if (seLat < -90 || seLat > 90 || seLong < -180 || seLong > 180)
        {
            throw new InvalidLocationException("Invalid coordinates");
        }
        if (nwLat <= seLat)
        {
            throw new InvalidLocationException("Northwest latitude must be greater than southeast latitude.");
        }
        if (nwLong >= seLong)
        {
            throw new InvalidLocationException("Northwest longitude must be less than southeast longitude.");
        }
    }
}