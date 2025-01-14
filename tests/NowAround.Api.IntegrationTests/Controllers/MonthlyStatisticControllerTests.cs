using System.Net;

namespace NowAround.Api.IntegrationTests.Controllers;

public class MonthlyStatisticControllerTests
{
    // GetMonthlyStatisticsByYearAsync Tests
    
    [Fact]
    public async Task GetMonthlyStatisticsByYearAsync_WithValidYear_ShouldReturnMonthlyStatistics()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");
        
        // Act
        var response = await client.GetAsync("/api/monthlyStatistic/2024");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);
    }
    
    [Fact]
    public async Task GetMonthlyStatisticsByYearAsync_WithFutureYear_ShouldReturnBadRequest()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");
        
        var year = DateTime.Now.Year + 1;
        
        // Act
        var response = await client.GetAsync("/api/monthlyStatistic/" + year);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetMonthlyStatisticsByYearAsync_WithInvalidYear_ShouldReturnInternalServerError()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");
        
        // Act
        var response = await client.GetAsync("/api/monthlyStatistic/invalid");
        
        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task GetMonthlyStatisticsByYearAsync_WithUnauthorizedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/monthlyStatistic/2024");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}