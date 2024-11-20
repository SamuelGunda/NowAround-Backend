using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.UnitTests;

internal class NowAroundWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string ConnectionString = "DataSource=:memory:";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(s =>
        {
            s.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            s.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(ConnectionString));

            var dbContext = CreateDbContext(s);
            
            var connection = dbContext.Database.GetDbConnection();
            connection.Open();
            dbContext.Database.EnsureCreated();
            
            SeedDatabase(dbContext);
        });
    }

    private static AppDbContext CreateDbContext(IServiceCollection s)
    {
        var serviceProvider = s.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return dbContext;
    }
    
    private void SeedDatabase(AppDbContext dbContext)
    {
        dbContext.Categories.Add(new Category { Name = "RESTAURANT" });
        dbContext.Categories.Add(new Category { Name = "BAR" });
        
        dbContext.Tags.Add(new Tag { Name = "PET_FRIENDLY" });
        dbContext.Tags.Add(new Tag { Name = "FAMILY_FRIENDLY" });
        
        dbContext.Establishments.Add(new Establishment
        {
            Auth0Id = "auth0|1234567890",
            Name = "Test Restaurant",
            Description = "Test Description",
            Address = "123 Test St",
            City = "Test City",
            Latitude = 0,
            Longitude = 0,
            PriceCategory = PriceCategory.Affordable,
            RequestStatus = RequestStatus.Accepted,
        });
        
        dbContext.EstablishmentCategories.Add(new EstablishmentCategory
        {
            EstablishmentId = 1,
            CategoryId = 1
        });
        
        dbContext.EstablishmentTags.Add(new EstablishmentTag
        {
            EstablishmentId = 1,
            TagId = 1
        });
        
        dbContext.SaveChanges();
    }
}