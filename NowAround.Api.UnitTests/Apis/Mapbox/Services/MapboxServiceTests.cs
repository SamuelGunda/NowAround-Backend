using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NowAround.Api.Apis.Mapbox.Services;

namespace NowAround.Api.UnitTests.Apis.Mapbox.Services;

public class MapboxServiceTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private MapboxService _mapboxService;
    
    public MapboxServiceTests()
    {
        Mock<IConfiguration> mockConfiguration = new();
        mockConfiguration.Setup(c => c["Mapbox:AccessToken"]).Returns("test-access-token");
        
        LoggerMock<MapboxService> logger = new();
        
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        _mapboxService = new MapboxService(new HttpClient(_mockHttpMessageHandler.Object), mockConfiguration.Object, logger.Object);
    }

    // GetCoordinatesFromAddressAsync tests
    
    [Fact]
    public async Task GetCoordinatesFromAddressAsync_ShouldReturnCoordinates_WhenResponseIsValid()
    {
        // Arrange
        const string address = "Some Street 123";
        const string postalCode = "12345";
        const string city = "Some City";
        var mockResponse = new
        {
            features = new[]
            {
                new
                {
                    geometry = new
                    {
                        coordinates = new[] { 19.12345, 48.12345 }
                    }
                }
            }
        };

        var jsonResponse = JsonConvert.SerializeObject(mockResponse);
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            });

        // Act
        var (lat, lng) = await _mapboxService.GetCoordinatesFromAddressAsync(address, postalCode, city);

        // Assert
        Assert.Equal(48.12345, lat);
        Assert.Equal(19.12345, lng);
    }

    [Fact]
    public void GetCoordinatesFromAddressAsync_ShouldThrowException_WhenResponseIsInvalid()
    {
        // Arrange
        const string address = "Some Street 123";
        const string postalCode = "12345";
        const string city = "Some City";

        var invalidResponse = new { invalid = "data" };
        var jsonResponse = JsonConvert.SerializeObject(invalidResponse);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            });

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _mapboxService.GetCoordinatesFromAddressAsync(address, postalCode, city));
        
        Assert.Equal("Unable to retrieve valid coordinates from the API response.", exception.Result.Message);
    }
    
    [Fact]
    public void GetCoordinatesFromAddressAsync_ShouldThrowException_WhenResponseIsNull()
    {
        // Arrange
        const string address = "Some Street 123";
        const string postalCode = "12345";
        const string city = "Some City";

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
            });

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _mapboxService.GetCoordinatesFromAddressAsync(address, postalCode, city));
        
        Assert.Equal("Unable to deserialize the API response.", exception.Result.Message);
    }
}
