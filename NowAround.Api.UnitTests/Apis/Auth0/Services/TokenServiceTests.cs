using System.Net;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NowAround.Api.Apis.Auth0.Services;
using NUnit.Framework;
using Assert = Xunit.Assert;

namespace NowAround.Api.UnitTests.Apis.Auth0.Services;

public class TokenServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IMemoryCache> _mockMemoryCache;
    private Mock<ILogger<TokenService>> _mockLogger;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private TokenService _tokenService;
    private string _auth0Domain = "test-domain.auth0.com";
    private string _clientId = "test-client-id";
    private string _clientSecret = "test-client-secret";
    private string _managementScopes = "test-scope";

    [SetUp]
    public void SetUp()
    {
        // Mock dependencies
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["Auth0:Domain"]).Returns(_auth0Domain);
        _mockConfiguration.Setup(c => c["Auth0:ClientId"]).Returns(_clientId);
        _mockConfiguration.Setup(c => c["Auth0:ClientSecret"]).Returns(_clientSecret);
        _mockConfiguration.Setup(c => c["Auth0:ManagementScopes"]).Returns(_managementScopes);

        _mockMemoryCache = new Mock<IMemoryCache>();
        _mockLogger = new Mock<ILogger<TokenService>>();

        // Mock the HttpClient behavior
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    access_token = "mock-access-token",
                    expires_in = 3600
                }), Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _tokenService = new TokenService(httpClient, _mockConfiguration.Object, _mockMemoryCache.Object, _mockLogger.Object);
    }
    
    [Test]
    public async Task GetManagementAccessTokenAsync_WhenNotAvailableInCache_ShouldReturnTokenFromApi()
    {
        // Arrange
        object cacheValue;
        _mockMemoryCache.Setup(m => m.TryGetValue("managementAccessToken", out cacheValue))
            .Returns(false);

        _mockMemoryCache.Setup(m => m.CreateEntry("managementAccessToken"))
            .Returns(new Mock<ICacheEntry>().Object);

        // Act
        var token = await _tokenService.GetManagementAccessTokenAsync();

        // Assert
        Assert.Equal("mock-access-token", token);
        _mockMemoryCache.Verify(m => m.CreateEntry("managementAccessToken"), Times.Once);
    }


    [Test]
    public async Task GetManagementAccessTokenAsync_WhenAvailable_ShouldReturnTokenFromCache()
    {
        // Arrange
        object cacheValue = "cached-token";
        _mockMemoryCache.Setup(m => m.TryGetValue("managementAccessToken", out cacheValue))
            .Returns(true);

        // Act:
        var token = await _tokenService.GetManagementAccessTokenAsync();

        // Assert
        Assert.Equal("cached-token", token);
    }

    [Test]
    public async Task GetManagementAccessTokenAsync_WhenApiFails_ShouldThrowException()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("{ \"error\": \"invalid_request\" }")
            });

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tokenService.GetManagementAccessTokenAsync());
    }
}