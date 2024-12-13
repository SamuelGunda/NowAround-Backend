using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Api.Apis.Auth0.Exceptions;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Apis.Mapbox.Interfaces;
using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Models.Responses;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Services;

namespace NowAround.Api.UnitTests.Services;

public class EstablishmentServiceTests
{
    
    private readonly Mock<IEstablishmentRepository> _establishmentRepositoryMock;
    private readonly Mock<IAuth0Service> _auth0ServiceMock;
    private readonly Mock<IMapboxService> _mapboxServiceMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;

    private readonly EstablishmentService _establishmentService;
    
    public EstablishmentServiceTests()
    {
        _establishmentRepositoryMock = new Mock<IEstablishmentRepository>();
        _auth0ServiceMock = new Mock<IAuth0Service>();
        _mapboxServiceMock = new Mock<IMapboxService>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        Mock<ILogger<EstablishmentService>> loggerMock = new();
        
        _establishmentService = new EstablishmentService(
            _auth0ServiceMock.Object,
            _mapboxServiceMock.Object,
            _establishmentRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _tagRepositoryMock.Object,
            loggerMock.Object);
    }
    
    // RegisterEstablishmentAsync Tests
    
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
                Tags = new List<string> { "Pet_Friendly" }
            },
            OwnerInfo = new OwnerInfo
            {
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com"
            }
        };
        var coordinates = (lat: 1.0, lng: 1.0);
        const string auth0Id = "auth0|123";
        var categories = new[] { new Category { Name = "Restaurant"} };
        var tags = new[] { new Tag { Name = "Pet_Friendly"} };
        
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsByPropertyAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(coordinates);
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<OwnerInfo>())).ReturnsAsync(auth0Id);
        _categoryRepositoryMock.Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>())).ReturnsAsync(categories[0]);
        _tagRepositoryMock
            .Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>()))
            .ReturnsAsync((string property, string value) =>
                tags.FirstOrDefault(t => t.Name == value));
        _establishmentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Establishment>())).ReturnsAsync(1);

        // Act
        await _establishmentService.RegisterEstablishmentAsync(establishmentRequest);

        // Assert
        _establishmentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Establishment>()), Times.Once);
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
                Tags = new List<string> { "Pet_Friendly" }
            },
            OwnerInfo = new OwnerInfo
            {
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com"
            }
        };

        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsByPropertyAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(true);

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
                Tags = new List<string> { "Pet_Friendly" }
            },
            OwnerInfo = new OwnerInfo
            {
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com"
            }
        };
        
        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsByPropertyAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
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
                Tags = new List<string> { "Pet_Friendly" }
            },
            OwnerInfo = new OwnerInfo
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            }
        };
        
        var categories = new[] { new Category { Name = "Restaurant"} };
        var tags = new[] { new Tag { Name = "Pet_Friendly"} };
        
        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsByPropertyAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((1.0, 1.0));
        _categoryRepositoryMock.Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>())).ReturnsAsync(categories[0]);
        _tagRepositoryMock
            .Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>()))
            .ReturnsAsync((string property, string value) =>
                tags.FirstOrDefault(t => t.Name == value));
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<OwnerInfo>())).ThrowsAsync(new Exception());
        
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
                Tags = new List<string> { "PET_FRIENDLY" }
            },
            OwnerInfo = new OwnerInfo
            {
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com"
            }
        };
        
        var coordinates = (lat: 1.0, lng: 1.0);
        const string auth0Id = "auth0|123";
        var categories = new[] { new Category { Name = "Restaurant"} };
        var tags = new[] { new Tag { Name = "PET_FRIENDLY"} };
        
        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsByPropertyAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(coordinates);
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<OwnerInfo>())).ReturnsAsync(auth0Id);
        _categoryRepositoryMock.Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>())).ReturnsAsync(categories[0]);
        _tagRepositoryMock
            .Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>()))
            .ReturnsAsync((string property, string value) =>
                tags.FirstOrDefault(t => t.Name == value));
        
        _establishmentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Establishment>())).ThrowsAsync(new Exception());
        
        await Assert.ThrowsAsync<Exception>(() => _establishmentService.RegisterEstablishmentAsync(establishmentRequest));
        
        _auth0ServiceMock.Verify(s => s.DeleteAccountAsync(auth0Id), Times.Once);
    }

    [Fact]
    public async Task RegisterEstablishmentAsync_InvalidCategoryThrowsException_ShouldThrowException()
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
                Category = new List<string> { "test" },
                Tags = new List<string> { "PET_FRIENDLY" }
            },
            OwnerInfo = new OwnerInfo
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            }
        };
        
        var coordinates = (lat: 1.0, lng: 1.0);
        const string auth0Id = "auth0|123";
        var tags = new[] { new Tag { Name = "PET_FRIENDLY"} };
        
        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsByPropertyAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(coordinates);
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<OwnerInfo>())).ReturnsAsync(auth0Id);
        _categoryRepositoryMock.Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>())).ReturnsAsync( null as Category);
        
        await Assert.ThrowsAsync<ArgumentException>(() => _establishmentService.RegisterEstablishmentAsync(establishmentRequest));
    }

    [Fact]
    public async Task RegisterEstablishmentAsync_InvalidTagThrowsException_ShouldThrowException()
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
                Category = new List<string> { "RESTAURANT" },
                Tags = new List<string> { "test" }
            },
            OwnerInfo = new OwnerInfo
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            }
        };
        
        var coordinates = (lat: 1.0, lng: 1.0);
        const string auth0Id = "auth0|123";
        var categories = new[] { new Category { Name = "RESTAURANT"} };
        
        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsByPropertyAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(coordinates);
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<OwnerInfo>())).ReturnsAsync(auth0Id);
        _categoryRepositoryMock.Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>())).ReturnsAsync(categories[0]);
        _tagRepositoryMock
            .Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>()))
            .ReturnsAsync(null as Tag);
        
        await Assert.ThrowsAsync<ArgumentException>(() => _establishmentService.RegisterEstablishmentAsync(establishmentRequest));
    }

    // GetEstablishmentByIdAsync tests

    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_ValidId_ShouldReturnEstablishmentDto()
    {
        // Arrange
        const int id = 1;
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
        _establishmentRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(establishment);
        
        // Act
        var result = await _establishmentService.GetEstablishmentByIdAsync(id);
        
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
    public async Task GetEstablishmentByAuth0IdAsync_InvalidId_ShouldThrowEstablishmentNotFoundException()
    {
        const int id = 999;
        _establishmentRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as Establishment);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _establishmentService.GetEstablishmentByIdAsync(id));
    }
    
    // GetEstablishmentByAuth0IdAsync tests
    
    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_ValidAuth0Id_ShouldReturnEstablishment()
    {
        // Arrange
        const string auth0Id = "test-auth0-id";
        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = "test-auth0-id",
            Name = "test-name",
            Description = "test-description",
            Website = "test-website",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            EstablishmentCategories = new List<EstablishmentCategory>(),
            EstablishmentTags = new List<EstablishmentTag>()
        };
        
        var establishmentProfile = new EstablishmentProfileResponse(
            establishment.Auth0Id,
            new GenericInfo(
                establishment.Name,
                "Default",
                establishment.Description,
                establishment.Website,
                "Moderate",
                new List<string>(),
                new List<string>(),
                new List<string>(),
                new List<SocialLinkDto>()
            ),
            new LocationInfo(
                establishment.Address,
                establishment.City,
                establishment.Longitude,
                establishment.Latitude,
                new BusinessHoursDto(
                    "08:00 - 17:00",
                    "08:00 - 17:00",
                    "08:00 - 17:00",
                    "08:00 - 17:00",
                    "08:00 - 17:00",
                    "08:00 - 17:00",
                    "08:00 - 14:00",
                    new List<BusinessHoursExceptionsDto>()
                )
            ),
            new List<PostWithAuthIdsResponse>(),
            new List<MenuDto>(),
            new RatingStatisticResponse(
                0, 0, 0, 0, 0,
                new List<ReviewWithAuthIdsResponse>()
                )
        );
        
        _establishmentRepositoryMock.Setup(r => r.GetByAuth0IdAsync(auth0Id)).ReturnsAsync(establishmentProfile);
        
        // Act
        var result = await _establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_InvalidAuth0Id_ShouldThrowEstablishmentNotFoundException()
    {
        const string auth0Id = "999";
        _establishmentRepositoryMock.Setup(r => r.GetByAuth0IdAsync(auth0Id)).ReturnsAsync(null as EstablishmentProfileResponse);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id));
    }
    
    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_RepositoryThrowsException_ShouldThrowException()
    {
        const string auth0Id = "test-auth0-id";
        _establishmentRepositoryMock.Setup(r => r.GetByAuth0IdAsync(auth0Id)).ThrowsAsync(new Exception());

        await Assert.ThrowsAsync<Exception>(() => _establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id));
    }
    
    // GetPendingEstablishmentsAsync tests
    
    [Fact]
    public async Task GetPendingEstablishmentsAsync_ShouldReturnListOfEstablishmentDto()
    {
        // Arrange
        var establishments = new List<Establishment>
        {
            new()
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
            }
        };
        _establishmentRepositoryMock.Setup(r => r.GetAllWhereRegisterStatusPendingAsync()).ReturnsAsync(establishments);
        
        // Act
        var result = await _establishmentService.GetPendingEstablishmentsAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(establishments.Count, result.Count);
    }
    
    // GetEstablishmentWithFilterAsync

    [Fact]
    public async Task GetEstablishmentsWithFilterAsync_ValidRequest_ShouldReturnListOfEstablishmentMarkerDto()
    {
        var establishmentDtos = new List<EstablishmentDto>
        {
            new()
            {
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
            }
        };
        
        SearchValues searchValues = new()
        {
            Name = "test-name",
            PriceCategory = 1,
            
            MapBounds = new MapBounds
            {
                        NwLat = 1.0,
                        NwLong = 1.0,
                        SeLat = 1.0,
                        SeLong = 1.0
            }
        };
        
        _establishmentRepositoryMock.Setup(r => r.GetRangeWithFilterAsync(searchValues, 0)).ReturnsAsync(establishmentDtos);
        
        var result = await _establishmentService.GetEstablishmentsWithFilterAsync(searchValues, 0);
        
        Assert.NotNull(result);
    }
    
    // GetEstablishmentsCountCreatedInMonthAsync tests
    
    [Fact]
    public async Task GetEstablishmentsCountCreatedInMonthAsync_ShouldReturnCorrectCount_WhenEstablishmentsExist()
    {
        
        // Arrange
        var monthStart = new DateTime(2023, 1, 1);
        var monthEnd = new DateTime(2023, 1, 31);
        const int expectedCount = 5;
        
        // Act
        _establishmentRepositoryMock.Setup(r => r.GetCountByCreatedAtBetweenDatesAsync(monthStart, monthEnd)).ReturnsAsync(expectedCount);
        var result = await _establishmentService.GetEstablishmentsCountCreatedInMonthAsync(monthStart, monthEnd);

        // Assert
        Assert.Equal(expectedCount, result);
    }
    
    // UpdateEstablishmentRegisterRequestAsync tests

    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_ValidInputs_ShouldCallUpdateAsync()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var requestStatus = RequestStatus.Accepted;

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<EstablishmentDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _establishmentService.UpdateEstablishmentRegisterRequestAsync(auth0Id, requestStatus);

        // Assert
        _establishmentRepositoryMock.Verify(
            r => r.UpdateAsync(It.Is<EstablishmentDto>(dto => 
                dto.Auth0Id == auth0Id && dto.RequestStatus == requestStatus)), 
            Times.Once);
    }
    
    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_RepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var auth0Id = "auth0|12345";
        var requestStatus = RequestStatus.Pending;

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<EstablishmentDto>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => 
            _establishmentService.UpdateEstablishmentRegisterRequestAsync(auth0Id, requestStatus));

        Assert.Equal("Database error", exception.Message);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<EstablishmentDto>()), Times.Once);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateEstablishmentRegisterRequestAsync_InvalidAuth0Id_ShouldThrowArgumentNullException(string auth0Id)
    {
        // Arrange
        var requestStatus = RequestStatus.Pending;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _establishmentService.UpdateEstablishmentRegisterRequestAsync(auth0Id, requestStatus));

        Assert.Equal("auth0Id", exception.ParamName);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<EstablishmentDto>()), Times.Never);
    }
    
    // UpdateEstablishmentAsync tests
    
    [Fact]
    public async Task UpdateEstablishmentAsync_ValidInput_ShouldCallUpdateAsync()
    {
        // Arrange
        var request = new EstablishmentUpdateRequest
        {
            Auth0Id = "auth0|123",
            Name = "Test Establishment",
            Description = "Updated description",
            PriceCategory = 2,
            Category = new List<string> { "Restaurant" },
            Tags = new List<string> { "PET_FRIENDLY" }
        };
    
        var categories = new[] { new Category { Name = "Restaurant" } };
        var tags = new[] { new Tag { Name = "PET_FRIENDLY" } };

        _categoryRepositoryMock.Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>())).ReturnsAsync(categories[0]);
        _tagRepositoryMock.Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>())).ReturnsAsync((string property, string value) =>
            tags.FirstOrDefault(t => t.Name == value));

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<EstablishmentDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _establishmentService.UpdateEstablishmentAsync(request);

        // Assert
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.Is<EstablishmentDto>(dto =>
            dto.Auth0Id == request.Auth0Id &&
            dto.Name == request.Name &&
            dto.Description == request.Description &&
            dto.PriceCategory == (PriceCategory)request.PriceCategory &&
            dto.EstablishmentCategories.Any(ec => ec.Category.Name == "Restaurant") &&
            dto.EstablishmentTags.Any(et => et.Tag.Name == "PET_FRIENDLY")
        )), Times.Once);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateEstablishmentAsync_InvalidAuth0Id_ShouldThrowArgumentNullException(string auth0Id)
    {
        // Arrange
        var request = new EstablishmentUpdateRequest
        {
            Auth0Id = auth0Id,
            Name = "Test Establishment",
            Description = "Updated description",
            PriceCategory = 2,
            Category = new List<string> { "Restaurant" },
            Tags = new List<string> { "PET_FRIENDLY" }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _establishmentService.UpdateEstablishmentAsync(request));

        Assert.Equal("Auth0Id", exception.ParamName);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<EstablishmentDto>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateEstablishmentAsync_InvalidCategoriesOrTags_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new EstablishmentUpdateRequest
        {
            Auth0Id = "auth0|123",
            Name = "Test Establishment",
            Description = "Updated description",
            PriceCategory = 2,
            Category = new List<string> { "NonExistentCategory" },
            Tags = new List<string> { "NonExistentTag" }
        };

        _categoryRepositoryMock.Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>())).ReturnsAsync((Category)null);
        _tagRepositoryMock.Setup(r => r.GetByPropertyAsync("Name", It.IsAny<string>())).ReturnsAsync((Tag)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _establishmentService.UpdateEstablishmentAsync(request));

        Assert.Contains("Category", exception.Message);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<EstablishmentDto>()), Times.Never);
    }
    
    // DeleteEstablishmentAsync tests
    
    [Fact]
    public async Task DeleteEstablishmentAsync_ValidAuth0Id_ShouldDeleteSuccessfully()
    {
        // Arrange
        const string auth0Id = "test-auth0-id";

        _auth0ServiceMock.Setup(s => s.DeleteAccountAsync(auth0Id)).Returns(Task.CompletedTask);
        _establishmentRepositoryMock.Setup(r => r.DeleteByAuth0IdAsync(auth0Id)).ReturnsAsync(true);

        // Act
        await _establishmentService.DeleteEstablishmentAsync(auth0Id);

        // Assert
        _auth0ServiceMock.Verify(s => s.DeleteAccountAsync(auth0Id), Times.Once);
        _establishmentRepositoryMock.Verify(r => r.DeleteByAuth0IdAsync(auth0Id), Times.Once);
    }
    
    [Fact]
    public async Task DeleteEstablishmentAsync_InvalidAuth0Id_ShouldThrowException()
    {
        // Arrange
        const string auth0Id = "test-auth0-id";

        _auth0ServiceMock.Setup(s => s.DeleteAccountAsync(auth0Id)).Returns(Task.CompletedTask);
        _establishmentRepositoryMock.Setup(r => r.DeleteByAuth0IdAsync(auth0Id)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _establishmentService.DeleteEstablishmentAsync(auth0Id));
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
        _establishmentRepositoryMock.Setup(r => r.DeleteByAuth0IdAsync(auth0Id)).ThrowsAsync(new Exception());

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