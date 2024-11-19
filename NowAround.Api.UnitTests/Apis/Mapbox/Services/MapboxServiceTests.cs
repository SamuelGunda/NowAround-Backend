using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using NowAround.Api.Apis.Mapbox.Services;
using Assert = Xunit.Assert;

namespace NowAround.Api.UnitTests.Apis.Mapbox.Services;

[TestFixture]
public class MapboxServiceTests
{
    private MapboxService _mapboxService;
    private LoggerMock<MapboxService> _logger;
    private IConfiguration _configuration;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;

    [SetUp]
    public void SetUp()
    {
        var configData = new Dictionary<string, string>
        {
            { "Mapbox:AccessToken", "mock-access-token" }
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _logger = new LoggerMock<MapboxService>();

        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        _mapboxService = new MapboxService(httpClient, _configuration, _logger.Object);
    }

    // GetCoordinatesFromAddressAsync tests
    
    [Test]
    public async Task GetCoordinatesFromAddressAsync_ShouldReturnCoordinates_WhenResponseIsValid()
    {
        // Arrange
        var address = "Some Street 123";
        var postalCode = "12345";
        var city = "Some City";
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

    [Test]
    public void GetCoordinatesFromAddressAsync_ShouldThrowException_WhenResponseIsInvalid()
    {
        // Arrange
        var address = "Some Street 123";
        var postalCode = "12345";
        var city = "Some City";

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
    
    [Test]
    public void GetCoordinatesFromAddressAsync_ShouldThrowException_WhenResponseIsNull()
    {
        // Arrange
        var address = "Some Street 123";
        var postalCode = "12345";
        var city = "Some City";

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
