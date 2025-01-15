using NowAround.Application.Common.Helpers;
using NowAround.Application.Dtos;
using NowAround.Domain.Enum;
using NowAround.Domain.Models;

namespace NowAround.Application.UnitTests.Common.Helpers;

public class EstablishmentSearchQueryBuilderTests
{
    [Fact]
    public void BuildSearchQuery_ShouldFilterByName()
    {
        // Arrange
        var establishments = GetTestEstablishments();
        var searchValues = new SearchValues { Name = "Cafe", MapBounds = new MapBounds() };

        // Act
        var query = EstablishmentSearchQueryBuilder.BuildSearchQuery(searchValues);
        var result = query(establishments.AsQueryable()).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Cozy Cafe", result.First().Name);
    }

    [Fact]
    public void BuildSearchQuery_ShouldFilterByPriceCategory()
    {
        // Arrange
        var establishments = GetTestEstablishments();
        var searchValues = new SearchValues { PriceCategory = 1, MapBounds = new MapBounds() };

        // Act
        var query = EstablishmentSearchQueryBuilder.BuildSearchQuery(searchValues);
        var result = query(establishments.AsQueryable()).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Bistro Delight", result.First().Name);
    }

    [Fact]
    public void BuildSearchQuery_ShouldFilterByCategoryName()
    {
        // Arrange
        var establishments = GetTestEstablishments();
        var searchValues = new SearchValues { CategoryName = "Restaurant", MapBounds = new MapBounds() };

        // Act
        var query = EstablishmentSearchQueryBuilder.BuildSearchQuery(searchValues);
        var result = query(establishments.AsQueryable()).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Bistro Delight", result.First().Name);
    }

    [Fact]
    public void BuildSearchQuery_ShouldFilterByTags()
    {
        // Arrange
        var establishments = GetTestEstablishments();
        var searchValues = new SearchValues { TagNames = new List<string> { "Cozy" }, MapBounds = new MapBounds() };

        // Act
        var query = EstablishmentSearchQueryBuilder.BuildSearchQuery(searchValues);
        var result = query(establishments.AsQueryable()).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Cozy Cafe", result.First().Name);
    }

    private static List<Establishment> GetTestEstablishments()
    {
        return
        [
            new Establishment
            {
                Name = "Cozy Cafe",
                PriceCategory = PriceCategory.Affordable,
                Latitude = 45,
                Longitude = -125,
                Categories = new List<Category> { new Category { Name = "Cafe" } },
                Tags = new List<Tag> { new Tag { Name = "Cozy" } },
                Address = "123 Main St",
                Auth0Id = "auth0|123",
                City = "Seattle"
            },

            new Establishment
            {
                Name = "Bistro Delight",
                PriceCategory = PriceCategory.Moderate,
                Latitude = 42,
                Longitude = -128,
                Categories = new List<Category> { new Category { Name = "Restaurant" } },
                Tags = new List<Tag> { new Tag { Name = "Fine Dining" } },
                Address = "456 Elm St",
                Auth0Id = "auth0|456",
                City = "Portland"
            },

            new Establishment
            {
                Name = "Fast Food Express",
                PriceCategory = PriceCategory.Expensive,
                Latitude = 35,
                Longitude = -100,
                Categories = new List<Category> { new Category { Name = "Fast Food" } },
                Tags = new List<Tag> { new Tag { Name = "Quick" } },
                Address = "789 Oak St",
                Auth0Id = "auth0|789",
                City = "San Francisco"
            }
        ];
    }
}