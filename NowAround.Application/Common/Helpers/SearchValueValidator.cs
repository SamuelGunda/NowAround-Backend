using NowAround.Application.Dtos;

namespace NowAround.Application.Common.Helpers;

public static class SearchValueValidator
{
    public static bool Validate(SearchValues searchValues)
    {
        return !string.IsNullOrWhiteSpace(searchValues.Name) || searchValues.PriceCategory.HasValue || !string.IsNullOrWhiteSpace(searchValues.CategoryName) || (searchValues.TagNames != null && searchValues.TagNames.Count != 0) || ValidateMapBounds(searchValues.MapBounds);
    }

    private static bool ValidateMapBounds(MapBounds mapBounds)
    {
        if (mapBounds is { NwLat: 0, NwLong: 0, SeLat: 0, SeLong: 0 })
        {
            return false;
        }

        return mapBounds.NwLat > mapBounds.SeLat && mapBounds.NwLong < mapBounds.SeLong;
    }
}