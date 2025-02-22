﻿using NowAround.Application.Dtos;
using NowAround.Domain.Enum;
using NowAround.Domain.Models;

namespace NowAround.Application.Common.Helpers;

public static class EstablishmentSearchQueryBuilder
{
    public static Func<IQueryable<Establishment>, IQueryable<Establishment>> BuildSearchQuery(SearchValues searchValues)
    {
        return query =>
        {
            var name = searchValues.Name;
            var priceCategory = searchValues.PriceCategory;
            var categoryName = searchValues.CategoryName;
            var tagNames = searchValues.TagNames;
            var mapBounds = searchValues.MapBounds;

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
                query = query.Where(e => e.PriceCategory == (PriceCategory)priceCategory);
            }

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                query = query.Where(e => e.Categories.Any(c => c.Name == categoryName));
            }

            if (tagNames != null && tagNames.Count != 0)
            {
                if (tagNames.Count == 1)
                {
                    query = query.Where(e => e.Tags.Any(t => tagNames.Contains(t.Name)));
                }
                else
                {
                    query = query.Where(e => tagNames.All(tag => e.Tags.Any(t => t.Name == tag)));
                }
            }

            return query;
        };
    }
}