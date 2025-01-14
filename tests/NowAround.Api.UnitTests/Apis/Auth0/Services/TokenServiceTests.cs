using System.Net;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NowAround.Api.Apis.Auth0.Services;

namespace NowAround.Api.UnitTests.Apis.Auth0.Services;

public class TokenServiceTests
{
    private Mock<IMemoryCache> _mockMemoryCache;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private TokenService _tokenService;
    
    public TokenServiceTests()
    {
        Mock<IConfiguration> mockConfiguration = new();
        mockConfiguration.Setup(c => c["Auth0:Domain"]).Returns("test-domain.auth0.com");
        mockConfiguration.Setup(c => c["Auth0:ClientId"]).Returns("test-client-id");
        mockConfiguration.Setup(c => c["Auth0:ClientSecret"]).Returns("test-client-secret");
        mockConfiguration.Setup(c => c["Auth0:ManagementScopes"]).Returns("test-scope");

        _mockMemoryCache = new Mock<IMemoryCache>();
        
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
        
        _tokenService = new TokenService(new HttpClient(_mockHttpMessageHandler.Object), mockConfiguration.Object, _mockMemoryCache.Object, Mock.Of<ILogger<TokenService>>());
    }
    
    [Fact]
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


    [Fact]
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

    [Fact]
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