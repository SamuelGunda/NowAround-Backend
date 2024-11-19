using Moq;
using System.Net;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Newtonsoft.Json;
using NowAround.Api.Apis.Auth0.Exceptions;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Apis.Auth0.Services;
using NUnit.Framework;
using Assert = Xunit.Assert;

namespace NowAround.Api.UnitTests.Apis.Auth0.Services;

[TestFixture]
public class Auth0ServiceTests
{
    private Mock<ITokenService> _mockTokenService;
    private Mock<IConfiguration> _mockConfiguration;
    private LoggerMock<Auth0Service> _logger;
    private string _auth0Domain;

    [SetUp]
    public void SetUp()
    {
        _mockTokenService = new Mock<ITokenService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _logger = new LoggerMock<Auth0Service>();

        _mockConfiguration.Setup(config => config["Auth0:Domain"]).Returns("test-domain.auth0.com");
        _mockConfiguration.Setup(config => config["Auth0:Roles:Establishment"]).Returns("establishment-role-id");
        _mockConfiguration.Setup(config => config["Auth0:Roles:User"]).Returns("user-role-id");

        _auth0Domain = "test-domain.auth0.com";
    }
    
    // RegisterEstablishmentAccountAsync tests
    
    [Test]
    public async Task RegisterEstablishmentAccountAsync_ShouldReturnAuth0UserId()
    {
        // Arrange
        var personalInfo = new PersonalInfo
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User"
        };
        
        var mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    user_id = "auth0|12345"
                }))
            });
        
        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new Auth0Service(httpClient, _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act
        var auth0Id = await service.RegisterEstablishmentAccountAsync(personalInfo);
        
        // Assert
        Assert.Equal("auth0|12345", auth0Id);
    }

    [Test]
    public async Task RegisterEstablishmentAccountAsync_WhenEmailIsAlreadyInUse_ShouldThrowEmailAlreadyInUseException()
    {
        // Arrange
        var personalInfo = new PersonalInfo
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User"
        };
        
        var mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Conflict));
        
        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new Auth0Service(httpClient, _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EmailAlreadyInUseException>(() => service.RegisterEstablishmentAccountAsync(personalInfo));
    }

    [Test]
    public async Task RegisterEstablishmentAccountAsync_WhenRequestFails_ShouldThrowHttpRequestException()
    {
        // Arrange
        var personalInfo = new PersonalInfo
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User"
        };

        var mockToken = "mock_access_token";

        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);

        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new Auth0Service(httpClient, _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.RegisterEstablishmentAccountAsync(personalInfo));
    }

    // GetEstablishmentOwnerFullNameAsync tests
    
    [Test]
    public async Task GetEstablishmentOwnerFullNameAsync_ShouldReturnFullName()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var mockToken = "mock_access_token";

        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);

        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    given_name = "Test",
                    family_name = "User"
                }))
            });

        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new Auth0Service(httpClient, _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);

        // Act
        var fullName = await service.GetEstablishmentOwnerFullNameAsync(auth0Id);

        // Assert
        Assert.Equal("Test User", fullName);
    }

    [Test]
    public async Task GetEstablishmentOwnerFullNameAsync_WhenUserNotFound_ShouldThrowHttpRequestException()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));
        
        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new Auth0Service(httpClient, _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetEstablishmentOwnerFullNameAsync(auth0Id));
    }

    [Test]
    public async Task GetEstablishmentOwnerFullNameAsync_WhenResponseCannotBeDeserialized_ShouldThrowJsonException()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("invalid-json")
            });
        
        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new Auth0Service(httpClient, _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<JsonReaderException>(() => service.GetEstablishmentOwnerFullNameAsync(auth0Id));
    }

    [Test]
    public async Task GetEstablishmentOwnerFullNameAsync_WhenAuth0IdIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var service = new Auth0Service(new HttpClient(), _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetEstablishmentOwnerFullNameAsync(null));
    }
    
    // DeleteAccountAsync tests

    [Test]
    public async Task DeleteAccountAsync_ShouldDeleteAccount()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));
        
        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new Auth0Service(httpClient, _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act
        await service.DeleteAccountAsync(auth0Id);
        
        // Assert
        Assert.True(true);
    }
    
    [Test]
    public async Task DeleteAccountAsync_WhenAuth0IdIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var service = new Auth0Service(new HttpClient(), _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.DeleteAccountAsync(null));
    }

    [Test]
    public async Task DeleteAccountAsync_WhenRequestFails_ShouldThrowHttpRequestException()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var mock = "mock_access_token";

        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mock);
        
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        
        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new Auth0Service(httpClient, _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.DeleteAccountAsync(auth0Id));
    }
    
    // AssignRoleAsync tests

    [Test]
    public async Task AssignRoleAsync_ShouldAssignRole()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var role = "user";
        var mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));
        
        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new Auth0Service(httpClient, _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act
        await service.AssignRoleAsync(auth0Id, role);
        
        // Assert
        Assert.True(true);
    }
    
    [Test]
    public async Task AssignRoleAsync_WhenAuth0IdIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var service = new Auth0Service(new HttpClient(), _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AssignRoleAsync(null, "user"));
    }
    
    [Test]
    public async Task AssignRoleAsync_WhenRoleIsInvalid_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new Auth0Service(new HttpClient(), _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AssignRoleAsync("auth0|12345", "invalid-role"));
    }

    [Test]
    public async Task AssignRoleAsync_WhenRequestFails_ShouldThrowHttpRequestException()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var role = "user";
        var mockToken = "mock_access_token";

        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);

        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new Auth0Service(httpClient, _mockTokenService.Object, _mockConfiguration.Object, _logger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.AssignRoleAsync(auth0Id, role));
    }
}
