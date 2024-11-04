using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Api.Apis.Auth0.Exceptions;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Apis.Mapbox.Interfaces;
using NowAround.Api.Exceptions;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;
using NowAround.Api.Services;

namespace NowAround.Api.UnitTests.Services;

public class EstablishmentServiceTests
{
    
    private readonly Mock<IEstablishmentRepository> _establishmentRepositoryMock;
    private readonly Mock<IAuth0Service> _auth0ServiceMock;
    private readonly Mock<IMapboxService> _mapboxServiceMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<ILogger<EstablishmentService>> _loggerMock;
    
    private readonly EstablishmentService _establishmentService;
    
    public EstablishmentServiceTests()
    {
        _establishmentRepositoryMock = new Mock<IEstablishmentRepository>();
        _auth0ServiceMock = new Mock<IAuth0Service>();
        _mapboxServiceMock = new Mock<IMapboxService>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        _loggerMock = new Mock<ILogger<EstablishmentService>>();
        
        _establishmentService = new EstablishmentService(
            _auth0ServiceMock.Object,
            _mapboxServiceMock.Object,
            _establishmentRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _loggerMock.Object);
    }
    
    // RegisterEstablishmentAsync tests
    
    [Fact]
    public async Task RegisterEstablishmentAsync_ValidRequest_ShouldReturnEstablishmentId()
    {
        // Arrange
        var establishmentRequest = new EstablishmentRegisterRequest
        {
            EstablishmentInfo = new EstablishmentInfo
            {
                Name = "Test Establishment", 
                Address = "123 Test St",
                PostalCode = "12345", 
                City = "Test City", 
                PriceCategory = 1, 
                Category = new List<string> { "Restaurant" }, 
                Tags = new List<string> { "Italian" }
            },
            PersonalInfo = new PersonalInfo
            {
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com"
            }
        };
        var coordinates = (lat: 1.0, lng: 1.0);
        const string auth0Id = "auth0|123";
        var categories = new[] { new Category { Name = "Restaurant"} };
        var tags = new[] { new Tag { Name = "Italian"} };
        
        _establishmentRepositoryMock.Setup(r => r.CheckIfEstablishmentExistsByNameAsync(establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(coordinates);
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<PersonalInfo>())).ReturnsAsync(auth0Id);
        _categoryRepositoryMock.Setup(r => r.GetCategoryByNameWithTagsAsync(It.IsAny<string>())).ReturnsAsync(categories[0]);
        _tagRepositoryMock.Setup(r => r.GetTagByNameAsync(It.IsAny<string>())).ReturnsAsync(tags[0]);
        _establishmentRepositoryMock.Setup(r => r.CreateEstablishmentAsync(It.IsAny<Establishment>())).ReturnsAsync(1);

        // Act
        var result = await _establishmentService.RegisterEstablishmentAsync(establishmentRequest);

        // Assert
        Assert.Equal(1, result);
    }
    
    [Fact]
    public async Task RegisterEstablishmentAsync_InvalidRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        var establishmentRequest = new EstablishmentRegisterRequest();
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _establishmentService.RegisterEstablishmentAsync(establishmentRequest));
    }
    
    [Fact]
    public async Task RegisterEstablishmentAsync_EstablishmentAlreadyExists_ShouldThrowEstablishmentAlreadyExistsException()
    {
        // Arrange
        var establishmentRequest = new EstablishmentRegisterRequest
        {
            EstablishmentInfo = new EstablishmentInfo
            {
                Name = "Test Establishment", 
                Address = "123 Test St",
                PostalCode = "12345",
                City = "Test City", 
                PriceCategory = 1, 
                Category = new List<string> { "Restaurant" }, 
                Tags = new List<string> { "Italian" }
            },
            PersonalInfo = new PersonalInfo
            {
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com"
            }
        };

        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfEstablishmentExistsByNameAsync(establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(true);

        await Assert.ThrowsAsync<EstablishmentAlreadyExistsException>(() => _establishmentService.RegisterEstablishmentAsync(establishmentRequest));
    }

    [Fact]
    public async Task RegisterEstablishmentAsync_MapboxServiceThrowsException_ShouldThrowException()
    {
        // Arrange
        var establishmentRequest = new EstablishmentRegisterRequest
        {
            EstablishmentInfo = new EstablishmentInfo
            {
                Name = "Test Establishment", 
                Address = "123 Test St", 
                City = "Test City",
                PostalCode = "12345",
                PriceCategory = 1, 
                Category = new List<string> { "Restaurant" }, 
                Tags = new List<string> { "Italian" }
            },
            PersonalInfo = new PersonalInfo
            {
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com"
            }
        };
        
        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfEstablishmentExistsByNameAsync(establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        
        await Assert.ThrowsAsync<Exception>(() => _establishmentService.RegisterEstablishmentAsync(establishmentRequest));
    }

    [Fact] 
    public async Task RegisterEstablishmentAsync_Auth0ServiceThrowsException_ShouldThrowException()
    {
        // Arrange
        var establishmentRequest = new EstablishmentRegisterRequest
        {
            EstablishmentInfo = new EstablishmentInfo
            {
                Name = "Test Establishment",
                Address = "123 Test St",
                City = "Test City",
                PostalCode = "12345",
                PriceCategory = 1,
                Category = new List<string> { "Restaurant" },
                Tags = new List<string> { "Italian" }
            },
            PersonalInfo = new PersonalInfo
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            }
        };
        
        var categories = new[] { new Category { Name = "Restaurant"} };
        var tags = new[] { new Tag { Name = "Italian"} };
        
        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfEstablishmentExistsByNameAsync(establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((1.0, 1.0));
        _categoryRepositoryMock.Setup(r => r.GetCategoryByNameWithTagsAsync(It.IsAny<string>())).ReturnsAsync(categories[0]);
        _tagRepositoryMock.Setup(r => r.GetTagByNameAsync(It.IsAny<string>())).ReturnsAsync(tags[0]);
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<PersonalInfo>())).ThrowsAsync(new Exception());
        
        await Assert.ThrowsAsync<Exception>(() => _establishmentService.RegisterEstablishmentAsync(establishmentRequest));
    }

    [Fact]
    public async Task RegisterEstablishmentAsync_EstablishmentRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var establishmentRequest = new EstablishmentRegisterRequest
        {
            EstablishmentInfo = new EstablishmentInfo
            {
                Name = "Test Establishment", 
                Address = "123 Test St", 
                City = "Test City",
                PostalCode = "12345",
                PriceCategory = 1, 
                Category = new List<string> { "Restaurant" }, 
                Tags = new List<string> { "Italian" }
            },
            PersonalInfo = new PersonalInfo
            {
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com"
            }
        };
        
        var coordinates = (lat: 1.0, lng: 1.0);
        const string auth0Id = "auth0|123";
        var categories = new[] { new Category { Name = "Restaurant"} };
        var tags = new[] { new Tag { Name = "Italian"} };
        
        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfEstablishmentExistsByNameAsync(establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(coordinates);
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<PersonalInfo>())).ReturnsAsync(auth0Id);
        _categoryRepositoryMock.Setup(r => r.GetCategoryByNameWithTagsAsync(It.IsAny<string>())).ReturnsAsync(categories[0]);
        _tagRepositoryMock.Setup(r => r.GetTagByNameAsync(It.IsAny<string>())).ReturnsAsync(tags[0]);
        
        _establishmentRepositoryMock.Setup(r => r.CreateEstablishmentAsync(It.IsAny<Establishment>())).ThrowsAsync(new Exception());
        
        await Assert.ThrowsAsync<Exception>(() => _establishmentService.RegisterEstablishmentAsync(establishmentRequest));
        
        _auth0ServiceMock.Verify(s => s.DeleteAccountAsync(auth0Id), Times.Once);
    }

    // GetEstablishmentByIdAsync tests
    
    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_ValidAuth0Id_ShouldReturnEstablishmentDto()
    {
        // Arrange
        const string auth0Id = "test-auth0-id";
        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = "test-auth0-id",
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            EstablishmentCategories = new List<EstablishmentCategory>(),
            EstablishmentTags = new List<EstablishmentTag>()
        };
        _establishmentRepositoryMock.Setup(r => r.GetEstablishmentByAuth0IdAsync(auth0Id)).ReturnsAsync(establishment);
        
        // Act
        var result = await _establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(establishment.Name, result.Name);
        Assert.Equal(establishment.Description, result.Description);
        Assert.Equal(establishment.City, result.City);
        Assert.Equal(establishment.Address, result.Address);
        Assert.Equal(establishment.Latitude, result.Latitude);
        Assert.Equal(establishment.Longitude, result.Longitude);
        Assert.Equal(establishment.PriceCategory, result.PriceCategory);
        Assert.Equal(establishment.RequestStatus, result.RequestStatus);
    }
    
    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_InvalidAuth0Id_ShouldThrowEstablishmentNotFoundException()
    {
        const string auth0Id = "999";
        _establishmentRepositoryMock.Setup(r => r.GetEstablishmentByAuth0IdAsync(auth0Id)).ReturnsAsync(null as Establishment);

        await Assert.ThrowsAsync<EstablishmentNotFoundException>(() => _establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id));
    }
    
    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_RepositoryThrowsException_ShouldThrowException()
    {
        const string auth0Id = "test-auth0-id";
        _establishmentRepositoryMock.Setup(r => r.GetEstablishmentByAuth0IdAsync(auth0Id)).ThrowsAsync(new Exception());

        await Assert.ThrowsAsync<Exception>(() => _establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id));
    }
    
    // DeleteEstablishmentAsync tests
    
    [Fact]
    public async Task DeleteEstablishmentAsync_ValidAuth0Id_ShouldDeleteSuccessfully()
    {
        // Arrange
        const string auth0Id = "test-auth0-id";

        _auth0ServiceMock.Setup(s => s.DeleteAccountAsync(auth0Id)).Returns(Task.CompletedTask);
        _establishmentRepositoryMock.Setup(r => r.DeleteEstablishmentByAuth0IdAsync(auth0Id)).ReturnsAsync(true);

        // Act
        await _establishmentService.DeleteEstablishmentAsync(auth0Id);

        // Assert
        _auth0ServiceMock.Verify(s => s.DeleteAccountAsync(auth0Id), Times.Once);
        _establishmentRepositoryMock.Verify(r => r.DeleteEstablishmentByAuth0IdAsync(auth0Id), Times.Once);
    }
    
    [Fact]
    public async Task DeleteEstablishmentAsync_InvalidAuth0Id_ShouldThrowException()
    {
        // Arrange
        const string auth0Id = "test-auth0-id";

        _auth0ServiceMock.Setup(s => s.DeleteAccountAsync(auth0Id)).Returns(Task.CompletedTask);
        _establishmentRepositoryMock.Setup(r => r.DeleteEstablishmentByAuth0IdAsync(auth0Id)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EstablishmentNotFoundException>(() => _establishmentService.DeleteEstablishmentAsync(auth0Id));
    }
    
    [Fact]
    public async Task DeleteEstablishmentAsync_Auth0ServiceThrowsException_ShouldThrowException()
    {
        // Arrange
        const string auth0Id = "test-auth0-id";

        _auth0ServiceMock.Setup(s => s.DeleteAccountAsync(auth0Id)).ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _establishmentService.DeleteEstablishmentAsync(auth0Id));
    }
    
    [Fact]
    public async Task DeleteEstablishmentAsync_RepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        const string auth0Id = "test-auth0-id";

        _auth0ServiceMock.Setup(s => s.DeleteAccountAsync(auth0Id)).Returns(Task.CompletedTask);
        _establishmentRepositoryMock.Setup(r => r.DeleteEstablishmentByAuth0IdAsync(auth0Id)).ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _establishmentService.DeleteEstablishmentAsync(auth0Id));
    }
    
    [Fact]
    public async Task DeleteEstablishmentAsync_Auth0IdIsNull_ShouldThrowException()
    {
        // Arrange
        const string auth0Id = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _establishmentService.DeleteEstablishmentAsync(auth0Id));
    }
}