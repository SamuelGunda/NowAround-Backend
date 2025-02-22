﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NowAround.Application.Interfaces;
using NowAround.Application.Services;
using NowAround.Domain.Enum;
using NowAround.Domain.Models;
using NowAround.Infrastructure.Context;

namespace NowAround.IntegrationTests;

internal class NowAroundWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Mock<IAuth0Service>? _auth0ServiceMock;
    private readonly Mock<IMapboxService>? _mapboxServiceMock;
    private readonly Mock<IMailService>? _mailServiceMock;

    public NowAroundWebApplicationFactory(
        Mock<IAuth0Service>? auth0ServiceMock = null,
        Mock<IMapboxService>? mapboxServiceMock = null,
        Mock<IMailService>? mailServiceMock = null)
    {
        _auth0ServiceMock = auth0ServiceMock;
        _mapboxServiceMock = mapboxServiceMock;
        _mailServiceMock = mailServiceMock;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Database
            var connection = new SqliteConnection("DataSource=:memory:");
            
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.AddSingleton(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options);
            
            connection.Open();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));

            var serviceProvider = services.BuildServiceProvider();
            var dbContextOptions = serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>();

            Console.WriteLine(dbContextOptions.Extensions
                .Select(e => e.GetType().Name)
                .Aggregate((current, next) => $"{current}, {next}"));
            
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
            SeedDatabase(dbContext);

            // Api Services
            if (_auth0ServiceMock != null)
            {
                services.RemoveAll<IAuth0Service>();
                services.AddSingleton(_auth0ServiceMock.Object);
            }

            if (_mapboxServiceMock != null)
            {
                services.RemoveAll<IMapboxService>();
                services.AddSingleton(_mapboxServiceMock.Object);
            }
            
            if (_mailServiceMock != null)
            {
                services.RemoveAll<IMailService>();
                services.AddSingleton(_mailServiceMock.Object);
            }

            // Authentication
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.TestAuthScheme;
                    options.DefaultChallengeScheme = TestAuthHandler.TestAuthScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.TestAuthScheme, options => { });

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
                .AddPolicy("EstablishmentOnly", policy => policy.RequireRole("Establishment"))
                .AddPolicy("UserOnly", policy => policy.RequireRole("User"));
        });
    }

    private static void SeedDatabase(AppDbContext dbContext)
    {
        // Categories
        var restaurantCategory = new Category { Name = "RESTAURANT" };
        var barCategory = new Category { Name = "BAR" };
        var cafeCategory = new Category { Name = "CAFE" };
        dbContext.Categories.AddRange(restaurantCategory, barCategory, cafeCategory);

        // Tags
        var petFriendlyTag = new Tag { Name = "PET_FRIENDLY" };
        var familyFriendlyTag = new Tag { Name = "FAMILY_FRIENDLY" };
        dbContext.Tags.AddRange(petFriendlyTag, familyFriendlyTag);

        // Establishments
        var establishmentRestaurant = new Establishment
        {
            Auth0Id = "auth0|valid",
            Name = "Test Restaurant",
            Description = "Test Description",
            Address = "123 Test St",
            City = "Test City",
            Latitude = 0,
            Longitude = 0,
            PriceCategory = PriceCategory.Affordable,
            Categories = new List<Category> { restaurantCategory },
            Tags = new List<Tag> { petFriendlyTag },
            RequestStatus = RequestStatus.Accepted,
            CreatedAt = new DateTime(2024, 1, 1)
        };

        var establishmentCafe = new Establishment
        {
            Auth0Id = "auth0|valid2",
            Name = "Test Cafe",
            Description = "Test Description",
            Address = "124 Test St",
            City = "Test City",
            Latitude = 2,
            Longitude = 2,
            PriceCategory = PriceCategory.Affordable,
            Categories = new List<Category> { cafeCategory },
            Tags = new List<Tag> { petFriendlyTag, familyFriendlyTag },
            RequestStatus = RequestStatus.Accepted,
            CreatedAt = new DateTime(2024, 1, 1)
        };

        dbContext.Establishments.AddRange(establishmentRestaurant, establishmentCafe);
        dbContext.SaveChanges();

        // Business Hours
        var businessHours = new BusinessHours
        {
            EstablishmentId = establishmentRestaurant.Id,
            Monday = "08:00-17:00",
            Tuesday = "08:00-17:00",
            Wednesday = "08:00-17:00",
            Thursday = "08:00-17:00",
            Friday = "08:00-17:00",
            Saturday = "08:00-17:00",
            Sunday = "08:00-17:00"
        };

        dbContext.BusinessHours.Add(businessHours);

        establishmentRestaurant.BusinessHours = businessHours;

        // Rating Statistic
        var ratingStatistic = new RatingStatistic
        {
            EstablishmentId = establishmentRestaurant.Id,
            OneStar = 0,
            TwoStars = 0,
            ThreeStars = 0,
            FourStars = 0,
            FiveStars = 0
        };

        dbContext.RatingStatistics.Add(ratingStatistic);

        establishmentRestaurant.RatingStatistic = ratingStatistic;

        // Users
        var user = new User { Auth0Id = "auth0|valid", FullName = "Test User" };
        dbContext.Users.Add(user);

        // Posts
        var post = new Post
        {
            EstablishmentId = establishmentRestaurant.Id,
            Headline = "Test Headline",
            Body = "Test Body",
        };

        dbContext.Posts.Add(post);
        dbContext.SaveChanges();
    }
}