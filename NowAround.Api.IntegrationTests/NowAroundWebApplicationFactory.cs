using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Apis.Mapbox.Interfaces;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.IntegrationTests;

internal class NowAroundWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Mock<IAuth0Service>? _auth0ServiceMock;
    private readonly Mock<IMapboxService>? _mapboxServiceMock;
    
    public NowAroundWebApplicationFactory(
        Mock<IAuth0Service>? auth0ServiceMock = null,
        Mock<IMapboxService>? mapboxServiceMock = null)
    {
        _auth0ServiceMock = auth0ServiceMock;
        _mapboxServiceMock = mapboxServiceMock;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            
            //Database
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));

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
        var restaurantCategory = new Category { Name = "RESTAURANT" };
        var barCategory = new Category { Name = "BAR" };
        var cafeCategory = new Category { Name = "CAFE" };
        dbContext.Categories.AddRange(restaurantCategory, barCategory, cafeCategory);

        var petFriendlyTag = new Tag { Name = "PET_FRIENDLY" };
        var familyFriendlyTag = new Tag { Name = "FAMILY_FRIENDLY" };
        dbContext.Tags.AddRange(petFriendlyTag, familyFriendlyTag);

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
            RequestStatus = RequestStatus.Accepted,
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
            RequestStatus = RequestStatus.Accepted,
        };
        
        dbContext.Establishments.AddRange(establishmentRestaurant, establishmentCafe);
        dbContext.SaveChanges();

        dbContext.EstablishmentCategories.Add(new EstablishmentCategory
        {
            EstablishmentId = establishmentRestaurant.Id,
            CategoryId = restaurantCategory.Id
        });
        
        dbContext.EstablishmentCategories.Add(new EstablishmentCategory
        {
            EstablishmentId = establishmentCafe.Id,
            CategoryId = cafeCategory.Id
        });

        dbContext.EstablishmentTags.Add(new EstablishmentTag
        {
            EstablishmentId = establishmentRestaurant.Id,
            TagId = petFriendlyTag.Id
        });
        
        dbContext.EstablishmentTags.Add(new EstablishmentTag
        {
            EstablishmentId = establishmentCafe.Id,
            TagId = petFriendlyTag.Id
        });
        
        dbContext.EstablishmentTags.Add(new EstablishmentTag
        {
            EstablishmentId = establishmentCafe.Id,
            TagId = familyFriendlyTag.Id
        });

        dbContext.SaveChanges();
    }
}