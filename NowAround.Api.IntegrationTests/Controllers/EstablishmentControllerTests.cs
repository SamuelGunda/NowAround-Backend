using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Responses;
using NowAround.Api.UnitTests;

namespace NowAround.Api.IntegrationTests.Controllers;

public class EstablishmentControllerTests
{
    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_WithValidAuth0Id_ReturnsEstablishment()
    {
        // Arrange
        var application = new NowAroundWebApplicationFactory();
            
        var auth0Id = "auth0|1234567890";
        var establishment = new EstablishmentResponse
        {
            Auth0Id = auth0Id,
            Name = "Test Establishment",
            Description = "Test Description",
            Address = "123 Test St",
            City = "Test City",
            Latitude = 0,
            Longitude = 0,
            PriceCategory = PriceCategory.Affordable,
            RequestStatus = RequestStatus.Accepted,
            CategoryNames = new List<string> { "Test Category" },
            TagNames = new List<string> { "Test Tag" }
        };

        var client = application.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/establishment/auth0Id/{auth0Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
    }
}