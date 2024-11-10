using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Utilities;

public static class EstablishmentFilterQueryBuilder
{
    public static IQueryable<Establishment> ApplyFilters(
        IQueryable<Establishment> query, 
        string? name,
        int? priceCategory,
        string? categoryName, 
        List<string>? tagNames)
    {
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(e => e.Name.Contains(name));
        }
        
        if (priceCategory.HasValue)
        {
            query = query.Where(e => e.PriceCategory == (PriceCategory) priceCategory);
        }
        
        if (!string.IsNullOrEmpty(categoryName))
        {
            query = query.Where(e => e.EstablishmentCategories.Any(ec => ec.Category.Name == categoryName));
        }

        if (tagNames != null && tagNames.Count != 0)
        {
            query = query.Where(e => e.EstablishmentTags.Any(et => tagNames.Contains(et.Tag.Name)));

        }
        return query;
    }
}