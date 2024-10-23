using NowAround.Api.Models.Domain;

namespace NowAround.Api.Builders;

public static class EstablishmentFilterQueryBuilder
{
    public static IQueryable<Establishment> ApplyFilters(
        IQueryable<Establishment> query, 
        string? name, 
        string? categoryName, 
        List<string>? tagNames)
    {
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(e => e.Name.Contains(name));
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