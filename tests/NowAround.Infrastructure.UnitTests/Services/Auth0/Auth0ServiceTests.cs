using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NowAround.Application.Common.Exceptions;
using NowAround.Application.Interfaces;
using NowAround.Application.Requests;
using NowAround.Application.Services;
using NowAround.Infrastructure.Services.Auth0;

namespace NowAround.Infrastructure.UnitTests.Services.Auth0;

public class Auth0ServiceTests
{
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IMailService> _mockMailService;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Auth0Service _auth0Service;
    
    public Auth0ServiceTests()
    {
        Mock<IConfiguration> mockConfiguration = new();
        mockConfiguration.Setup(c => c["Auth0:ClientSecret"]).Returns("test-client-secret");
        mockConfiguration.Setup(c => c["Auth0:ClientId"]).Returns("test-client-id");
        mockConfiguration.Setup(c => c["Auth0:Domain"]).Returns("test-domain.auth0.com");
        mockConfiguration.Setup(c => c["Auth0:Roles:Establishment"]).Returns("establishment-role-id");
        mockConfiguration.Setup(c => c["Auth0:Roles:User"]).Returns("user-role-id");
        
        _mockTokenService = new Mock<ITokenService>();
        _mockMailService = new Mock<IMailService>();
        
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        
        _auth0Service = new Auth0Service(new HttpClient(_mockHttpMessageHandler.Object), _mockTokenService.Object, _mockMailService.Object, mockConfiguration.Object, Mock.Of<ILogger<Auth0Service>>());
    }
    
    // RegisterEstablishmentAccountAsync tests
    
    [Fact]
    public async Task RegisterEstablishmentAccountAsync_ShouldReturnAuth0UserId()
    {
        // Arrange
        var personalInfo = new EstablishmentOwnerInfo
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User"
        };
        
        const string mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        _mockHttpMessageHandler
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
        
        // Act
        var auth0Id = await _auth0Service.RegisterEstablishmentAccountAsync(personalInfo);
        
        // Assert
        Assert.Equal("auth0|12345", auth0Id);
    }

    [Fact]
    public async Task RegisterEstablishmentAccountAsync_WhenEmailIsAlreadyInUse_ShouldThrowEmailAlreadyInUseException()
    {
        // Arrange
        var personalInfo = new EstablishmentOwnerInfo
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User"
        };
        
        var mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Conflict));
        
   
        // Act & Assert
        await Assert.ThrowsAsync<EmailAlreadyInUseException>(() => _auth0Service.RegisterEstablishmentAccountAsync(personalInfo));
    }

    [Fact]
    public async Task RegisterEstablishmentAccountAsync_WhenRequestFails_ShouldThrowHttpRequestException()
    {
        // Arrange
        var personalInfo = new EstablishmentOwnerInfo
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User"
        };

        const string mockToken = "mock_access_token";

        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _auth0Service.RegisterEstablishmentAccountAsync(personalInfo));
    }

    // GetEstablishmentOwnerFullNameAsync tests
    
    [Fact]
    public async Task GetEstablishmentOwnerFullNameAsync_ShouldReturnFullName()
    {
        // Arrange
        const string auth0Id = "auth0|12345";
        const string mockToken = "mock_access_token";

        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);

        _mockHttpMessageHandler
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
                    family_name = "User",
                    email = "Test@test.sk"
                }))
            });
        
        // Act
        var fullName = await _auth0Service.GetEstablishmentOwnerFullNameAndEmailAsync(auth0Id);

        // Assert
        Assert.Equal(("Test User", "Test@test.sk"), fullName);        
    }

    [Fact]
    public async Task GetEstablishmentOwnerFullNameAsync_WhenUserNotFound_ShouldThrowHttpRequestException()
    {
        // Arrange
        const string auth0Id = "auth0|12345";
        const string mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));
        
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _auth0Service.GetEstablishmentOwnerFullNameAndEmailAsync(auth0Id));
    }

    [Fact]
    public async Task GetEstablishmentOwnerFullNameAsync_WhenResponseCannotBeDeserialized_ShouldThrowJsonException()
    {
        // Arrange
        const string auth0Id = "auth0|12345";
        const string mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        _mockHttpMessageHandler            
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

        // Act & Assert
        await Assert.ThrowsAsync<JsonReaderException>(() => _auth0Service.GetEstablishmentOwnerFullNameAndEmailAsync(auth0Id));
    }

    [Fact]
    public async Task GetEstablishmentOwnerFullNameAsync_WhenAuth0IdIsNull_ShouldThrowArgumentNullException()
    {

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _auth0Service.GetEstablishmentOwnerFullNameAndEmailAsync(null));
    }
    
    // ChangeAccountPasswordAsync tests
    
    [Fact]
    public async Task ChangeAccountPasswordAsync_ShouldChangePasswordSuccessfully()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var newPassword = "new_secure_password";
        var mockToken = "mock_access_token";

        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Patch &&
                    req.RequestUri.ToString() == $"https://test-domain.auth0.com/api/v2/users/{auth0Id}"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _auth0Service.ChangeAccountPasswordAsync(auth0Id, newPassword);

        // Assert
        _mockHttpMessageHandler
            .Protected()
            .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task ChangeAccountPasswordAsync_WhenRequestFails_ShouldThrowHttpRequestException()
    {
        // Arrange
        const string auth0Id = "auth0|12345";
        const string newPassword = "new_secure_password";
        const string mockToken = "mock_access_token";

        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Patch &&
                    req.RequestUri.ToString() == $"https://test-domain.auth0.com/api/v2/users/{auth0Id}"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _auth0Service.ChangeAccountPasswordAsync(auth0Id, newPassword));
    }
    
    // VerifyOldPasswordAsync tests

    [Fact]
    public async Task VerifyOldPasswordAsync_WhenLoginFails_ShouldReturnFalse()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var oldPassword = "wrong_password";

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString() == $"https://test-domain.auth0.com/oauth/token"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        // Act
        var isValid = await _auth0Service.VerifyOldPasswordAsync(auth0Id, oldPassword);

        // Assert
        Assert.False(isValid);
    }

    
    // DeleteAccountAsync tests

    [Fact]
    public async Task DeleteAccountAsync_ShouldDeleteAccount()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

        // Act
        await _auth0Service.DeleteAccountAsync(auth0Id);
        
        // Assert
        Assert.True(true);
    }
    
    [Fact]
    public async Task DeleteAccountAsync_WhenAuth0IdIsNull_ShouldThrowArgumentNullException()
    {

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _auth0Service.DeleteAccountAsync(null));
    }

    [Fact]
    public async Task DeleteAccountAsync_WhenRequestFails_ShouldThrowHttpRequestException()
    {
        // Arrange
        const string auth0Id = "auth0|12345";
        const string mock = "mock_access_token";

        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mock);
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _auth0Service.DeleteAccountAsync(auth0Id));
    }
    
    // AssignRoleAsync tests

    [Fact]
    public async Task AssignRoleAsync_ShouldAssignRole()
    {
        // Arrange
        const string auth0Id = "auth0|12345";
        const string role = "user";
        const string mockToken = "mock_access_token";
        
        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

        // Act
        await _auth0Service.AssignRoleAsync(auth0Id, role);
        
        // Assert
        Assert.True(true);
    }
    
    [Fact]
    public async Task AssignRoleAsync_WhenAuth0IdIsNull_ShouldThrowArgumentNullException()
    {

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _auth0Service.AssignRoleAsync(null, "user"));
    }
    
    [Fact]
    public async Task AssignRoleAsync_WhenRoleIsInvalid_ShouldThrowArgumentException()
    {

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _auth0Service.AssignRoleAsync("auth0|12345", "invalid-role"));
    }

    [Fact]
    public async Task AssignRoleAsync_WhenRequestFails_ShouldThrowHttpRequestException()
    {
        // Arrange
        const string auth0Id = "auth0|12345";
        const string role = "user";
        const string mockToken = "mock_access_token";

        _mockTokenService
            .Setup(service => service.GetManagementAccessTokenAsync())
            .ReturnsAsync(mockToken);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _auth0Service.AssignRoleAsync(auth0Id, role));
    }
}
