using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NowAround.Infrastructure.Services.Mapbox;

namespace NowAround.Infrastructure.UnitTests.Services.Mapbox;

public class MapboxServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly MapboxService _mapboxService;
    
    public MapboxServiceTests()
    {
        Mock<IConfiguration> mockConfiguration = new();
        mockConfiguration.Setup(c => c["Mapbox:AccessToken"]).Returns("test-access-token");
        
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        _mapboxService = new MapboxService(new HttpClient(_mockHttpMessageHandler.Object), mockConfiguration.Object, Mock.Of<ILogger<MapboxService>>());
    }

    // GetCoordinatesFromAddressAsync tests
    
    [Fact]
    public async Task GetCoordinatesFromAddressAsync_WhenResponseIsValid_ShouldReturnCoordinates()
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
    public void GetCoordinatesFromAddressAsync_WhenResponseIsInvalid_ShouldThrowException()
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
    public void GetCoordinatesFromAddressAsync_WhenResponseIsNull_ShouldThrowException()
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
    
    // GetAddressFromCoordinatesAsync tests
    
    [Fact]
    public async Task GetAddressFromCoordinatesAsync_ShouldReturnAddressAndCity_WhenResponseIsValid()
    {
        // Arrange
        const double lat = 48.12345;
        const double lng = 19.12345;
        var mockResponse = new
        {
            features = new[]
            {
                new
                {
                    place_name = "Some Street 123, Some City"
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
        var (address, city) = await _mapboxService.GetAddressFromCoordinatesAsync(lat, lng);

        // Assert
        Assert.Equal("Some Street 123", address);
        Assert.Equal("Some City", city);
    }

    [Fact]
    public void GetAddressFromCoordinatesAsync_ShouldThrowException_WhenResponseIsInvalid()
    {
        // Arrange
        const double lat = 48.12345;
        const double lng = 19.12345;
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
            async () => await _mapboxService.GetAddressFromCoordinatesAsync(lat, lng));
        
        Assert.Equal("Unable to retrieve address from the API response.", exception.Result.Message);
    }

    [Fact]
    public void GetAddressFromCoordinatesAsync_ShouldThrowException_WhenResponseIsNull()
    {
        // Arrange
        const double lat = 48.12345;
        const double lng = 19.12345;

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
            async () => await _mapboxService.GetAddressFromCoordinatesAsync(lat, lng));
        
        Assert.Equal("Unable to deserialize the API response.", exception.Result.Message);
    }

    [Fact]
    public void GetAddressFromCoordinatesAsync_ShouldThrowException_WhenPlaceNameIsEmpty()
    {
        // Arrange
        const double lat = 48.12345;
        const double lng = 19.12345;
        var mockResponse = new
        {
            features = new[]
            {
                new { place_name = string.Empty }
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

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _mapboxService.GetAddressFromCoordinatesAsync(lat, lng));
        
        Assert.Equal("Unable to retrieve address from the API response.", exception.Result.Message);
    }

    [Fact]
    public void GetAddressFromCoordinatesAsync_ShouldThrowException_WhenPlaceNameFormatIsUnexpected()
    {
        // Arrange
        const double lat = 48.12345;
        const double lng = 19.12345;
        var mockResponse = new
        {
            features = new[]
            {
                new { place_name = "UnexpectedFormat" }
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

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _mapboxService.GetAddressFromCoordinatesAsync(lat, lng));
        
        Assert.Equal("Unable to parse the place name.", exception.Result.Message);
    }
}
