using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Utilities;

public static class EstablishmentFilteredSearchQueryBuilder
{
    public static IQueryable<Establishment> ApplyFilters(IQueryable<Establishment> query, FilterValues filterValues)
    {
        
        var name = filterValues.Name;
        var priceCategory = filterValues.PriceCategory;
        var categoryName = filterValues.CategoryName;
        var tagNames = filterValues.TagNames;
        var mapBounds = filterValues.MapBounds;
        
        if (mapBounds is not { NwLat: 0, NwLong: 0, SeLat: 0, SeLong: 0 })
        {
            query = query
                .Where(e => e.Latitude <= mapBounds.NwLat && e.Latitude >= mapBounds.SeLat)
                .Where(e => e.Longitude >= mapBounds.NwLong && e.Longitude <= mapBounds.SeLong);
        }
        
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(e => e.Name.Contains(name));
        }
        
        if (priceCategory.HasValue)
        {
            query = query.Where(e => e.PriceCategory == (PriceCategory) priceCategory);
        }
        
        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            query = query.Where(e => e.EstablishmentCategories.Any(ec => ec.Category.Name == categoryName));
        }

        if (tagNames != null && tagNames.Count != 0)
        {
            if (tagNames.Count == 1)
            {
                query = query.Where(e => e.EstablishmentTags.Any(et => tagNames.Contains(et.Tag.Name)));
            }
            else
            {
                query = query.Where(e => tagNames.All(tag => e.EstablishmentTags.Any(et => et.Tag.Name == tag)));
            }
        }
        return query;
    }
}