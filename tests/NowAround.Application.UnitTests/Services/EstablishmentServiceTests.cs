using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Application.Common.Exceptions;
using NowAround.Application.Dtos;
using NowAround.Application.Interfaces;
using NowAround.Application.Requests;
using NowAround.Application.Responses;
using NowAround.Application.Services;
using NowAround.Domain.Enum;
using NowAround.Domain.Interfaces.Base;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;

namespace NowAround.Application.UnitTests.Services;

public class EstablishmentServiceTests
{
    
    private readonly Mock<IEstablishmentRepository> _establishmentRepositoryMock;
    private readonly Mock<IAuth0Service> _auth0ServiceMock;
    private readonly Mock<IMapboxService> _mapboxServiceMock;
    private readonly Mock<IBaseRepository<Category>> _categoryRepositoryMock;
    private readonly Mock<IBaseRepository<Tag>> _tagRepositoryMock;
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly Mock<IMailService> _mailServiceMock;

    private readonly EstablishmentService _establishmentService;
    
    public EstablishmentServiceTests()
    {
        _establishmentRepositoryMock = new Mock<IEstablishmentRepository>();
        _auth0ServiceMock = new Mock<IAuth0Service>();
        _mapboxServiceMock = new Mock<IMapboxService>();
        _categoryRepositoryMock = new Mock<IBaseRepository<Category>>();
        _tagRepositoryMock = new Mock<IBaseRepository<Tag>>();
        _storageServiceMock = new Mock<IStorageService>();
        _mailServiceMock = new Mock<IMailService>();
        Mock<ILogger<EstablishmentService>> loggerMock = new();
        
        _establishmentService = new EstablishmentService(
            _auth0ServiceMock.Object,
            _mapboxServiceMock.Object,
            _establishmentRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _storageServiceMock.Object,
            _mailServiceMock.Object,
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
            EstablishmentOwnerInfo = new EstablishmentOwnerInfo
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
        
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(coordinates);
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<EstablishmentOwnerInfo>())).ReturnsAsync(auth0Id);
        _categoryRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), false)).ReturnsAsync(categories[0]);
        _tagRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tag, bool>>>(), false)).ReturnsAsync(tags[0]);
        _establishmentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Establishment>())).ReturnsAsync(1);

        // Act
        await _establishmentService.RegisterEstablishmentAsync(establishmentRequest);

        // Assert
        _establishmentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Establishment>()), Times.Once);
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
            EstablishmentOwnerInfo = new EstablishmentOwnerInfo
            {
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com"
            }
        };

        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(true);

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _establishmentService.RegisterEstablishmentAsync(establishmentRequest));
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
            EstablishmentOwnerInfo = new EstablishmentOwnerInfo
            {
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com"
            }
        };
        
        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
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
            EstablishmentOwnerInfo = new EstablishmentOwnerInfo
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            }
        };
        
        var categories = new[] { new Category { Name = "Restaurant"} };
        var tags = new[] { new Tag { Name = "Pet_Friendly"} };
        
        // Act & Assert
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((1.0, 1.0));
        _categoryRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), false)).ReturnsAsync(categories[0]);
        _tagRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tag, bool>>>(), false)).ReturnsAsync(tags[0]);
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<EstablishmentOwnerInfo>())).ThrowsAsync(new Exception());
        
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
            EstablishmentOwnerInfo = new EstablishmentOwnerInfo
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
        _establishmentRepositoryMock.Setup(r => r.CheckIfExistsAsync("Name", establishmentRequest.EstablishmentInfo.Name)).ReturnsAsync(false);
        _mapboxServiceMock.Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(coordinates);
        _auth0ServiceMock.Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<EstablishmentOwnerInfo>())).ReturnsAsync(auth0Id);
        _categoryRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), false)).ReturnsAsync(categories[0]);
        _tagRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tag, bool>>>(), false)).ReturnsAsync(tags[0]);
        
        _establishmentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Establishment>())).ThrowsAsync(new Exception());
        
        await Assert.ThrowsAsync<Exception>(() => _establishmentService.RegisterEstablishmentAsync(establishmentRequest));
        
        _auth0ServiceMock.Verify(s => s.DeleteAccountAsync(auth0Id), Times.Once);
    }

    // GetEstablishmentByAuth0IdAsync tests

    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_ValidId_ShouldReturnEstablishmentDto()
    {
        // Arrange
        const string auth0Id = "test-auth0-id";
        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>()
        };
        _establishmentRepositoryMock.Setup(r => r.GetAsync(e => e.Auth0Id == auth0Id, false)).ReturnsAsync(establishment);
        
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
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>()
        };
        
        _establishmentRepositoryMock.Setup(r => r.GetProfileByAuth0IdAsync(auth0Id)).ReturnsAsync(establishment);
        
        // Act
        var result = await _establishmentService.GetEstablishmentProfileByAuth0IdAsync(auth0Id);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_RepositoryThrowsException_ShouldThrowException()
    {
        const string auth0Id = "test-auth0-id";
        _establishmentRepositoryMock.Setup(r => r.GetProfileByAuth0IdAsync(auth0Id)).ThrowsAsync(new Exception());

        await Assert.ThrowsAsync<Exception>(() => _establishmentService.GetEstablishmentProfileByAuth0IdAsync(auth0Id));
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
                Categories= new List<Category>(),
                Tags = new List<Tag>()
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
    
    // GetEstablishmentWithFilterAsync tests

    [Fact]
    public async Task GetEstablishmentsWithFilterAsync_WhenSearchValuesAreValid_ShouldReturnEstablishments()
    {
        // Arrange
        var searchValues = new SearchValues
        {
            Name = "Test Establishment",
            PriceCategory = 1,
            CategoryName = "Restaurant",
            TagNames = ["Pet_Friendly"],
            MapBounds = new MapBounds { NwLat = 10, NwLong = 20, SeLat = 5, SeLong = 25 }
        };
        const int page = 1;
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
                Categories = new List<Category>(),
                Tags = new List<Tag>()
            },
            
            new()
            {
                Id = 2,
                Auth0Id = "test-auth0-id",
                Name = "test-name",
                Description = "test-description",
                City = "test-city",
                Address = "test-address",
                Latitude = 1.0,
                Longitude = 1.0,
                PriceCategory = PriceCategory.Moderate,
                RequestStatus = RequestStatus.Pending,
                Categories = new List<Category>(),
                Tags = new List<Tag>()
            }
        };
        
        _establishmentRepositoryMock.Setup(r => r.GetRangeWithFilterAsync(It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>>(), page))
            .ReturnsAsync(establishments);

        // Act
        var result = await _establishmentService.GetEstablishmentsWithFilterAsync(searchValues, page);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(establishments.Count, result.Count);
    }

    [Fact]
    public void GetEstablishmentsWithFilterAsync_WhenSearchValuesAreInvalid_ShouldThrowException()
    {
        // Arrange
        var searchValues = new SearchValues
        {
            Name = null,
            PriceCategory = null,
            CategoryName = null,
            TagNames = null,
            MapBounds = new MapBounds { NwLat = 0, NwLong = 0, SeLat = 0, SeLong = 0 }
        };
        var page = 1;

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidSearchActionException>(
            async () => await _establishmentService.GetEstablishmentsWithFilterAsync(searchValues, page));

        Assert.Equal("Invalid search values", exception.Result.Message);
    }

    [Fact]
    public void GetEstablishmentsWithFilterAsync_WhenPageIsInvalid_ShouldThrowException()
    {
        // Arrange
        var searchValues = new SearchValues
        {
            Name = "Test Establishment",
            PriceCategory = 1,
            CategoryName = "Restaurant",
            TagNames = ["Pet_Friendly"],
            MapBounds = new MapBounds { NwLat = 10, NwLong = 20, SeLat = 5, SeLong = 25 }
        };
        const int page = -1;

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidSearchActionException>(
            async () => await _establishmentService.GetEstablishmentsWithFilterAsync(searchValues, page));

        Assert.Equal("Page must be greater than 0", exception.Result.Message);
    }

    [Fact]
    public async Task GetEstablishmentsWithFilterAsync_WhenNoResultsFromRepository_ShouldReturnEmptyList()
    {
        // Arrange
        var searchValues = new SearchValues
        {
            Name = "Test Establishment",
            PriceCategory = 1,
            CategoryName = "Restaurant",
            TagNames = ["Pet_Friendly"],
            MapBounds = new MapBounds { NwLat = 10, NwLong = 20, SeLat = 5, SeLong = 25 }
        };
        const int page = 1;
        
        _establishmentRepositoryMock.Setup(r => r.GetRangeWithFilterAsync(It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>>(), page))
            .ReturnsAsync([]);

        // Act
        var result = await _establishmentService.GetEstablishmentsWithFilterAsync(searchValues, page);

        // Assert
        Assert.Empty(result);
    }
    
    // GetEstablishmentsCountCreatedInMonthAsync tests
    
    [Fact]
    public async Task GetEstablishmentsCountCreatedInMonthAsync_WhenEstablishmentsExist_ShouldReturnCorrectCount()
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

    // UpdateEstablishmentGenericInfoAsync tests
    
    [Fact]
    public async Task UpdateEstablishmentGenericInfoAsync_ValidRequest_ShouldUpdateAndReturnGenericInfo()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var request = new EstablishmentGenericInfoUpdateRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            PriceCategory = 2,
            Categories = new List<string> { "Cafe" },
            Tags = new List<string> { "Outdoor_Seating" }
        };
        
        var establishment = new Establishment
        {
            Auth0Id = auth0Id,
            Name = "Old Name",
            Description = "Old Description",
            Address = "Test Address",
            City = "Test City",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            Categories = new List<Category> { new() { Name = "Restaurant" } },
            Tags = new List<Tag> { new() { Name = "Pet_Friendly" } },
            SocialLinks = new List<SocialLink> { new() { Name = "Facebook", Url = "http://facebook.com/old" } }
        };
        
        var categories = new[] { new Category { Name = "Cafe" } };
        var tags = new[] { new Tag { Name = "Outdoor_Seating" } };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);
        _categoryRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync(categories[0]);
        _tagRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tag, bool>>>(), true))
            .ReturnsAsync(tags[0]);
        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _establishmentService.UpdateEstablishmentGenericInfoAsync(auth0Id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal("Expensive", result.PriceRange);
        Assert.Contains("Cafe", result.Categories);
        Assert.Contains("Outdoor_Seating", result.Tags);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateEstablishmentGenericInfoAsync_EstablishmentNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var request = new EstablishmentGenericInfoUpdateRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            PriceCategory = 2,
            Categories = new List<string> { "Category1" },
            Tags = new List<string> { "Tag1" }
        };

        _establishmentRepositoryMock.Setup(r => r.GetAsync(e => e.Auth0Id == auth0Id, true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ThrowsAsync(new EntityNotFoundException("Establishment not found"));

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _establishmentService.UpdateEstablishmentGenericInfoAsync(auth0Id, request));
    }

    [Fact]
    public async Task UpdateEstablishmentGenericInfoAsync_UpdateFails_ShouldThrowException()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var request = new EstablishmentGenericInfoUpdateRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            PriceCategory = 2,
            Categories = new List<string> { "Category1", "Category2" },
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = auth0Id,
            Name = "Old Name",
            Description = "Old Description",
            Address = "Test Address",
            City = "Test City",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            SocialLinks = new List<SocialLink>()
        };

        _establishmentRepositoryMock.Setup(r => r.GetAsync(e => e.Auth0Id == auth0Id, true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        _establishmentRepositoryMock.Setup(r => r.UpdateAsync(establishment))
            .ThrowsAsync(new Exception("Database update failed"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _establishmentService.UpdateEstablishmentGenericInfoAsync(auth0Id, request));
    }
    
    // UpdateEstablishmentLocationInfoAsync tests
    
    [Fact]
    public async Task UpdateEstablishmentLocationInfoAsync_ValidRequest_ShouldUpdateAndReturnLocationInfo()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var request = new EstablishmentLocationInfoUpdateRequest
        {
            Lat = 40.7128,
            Long = -74.0060,
            BusinessHours = new BusinessHoursDto(
            
                Monday: "9:00 AM - 5:00 PM",
                Tuesday: "9:00 AM - 5:00 PM",
                Wednesday: "9:00 AM - 5:00 PM",
                Thursday: "9:00 AM - 5:00 PM",
                Friday: "9:00 AM - 5:00 PM",
                Saturday: "Closed",
                Sunday: "Closed",
                BusinessHoursExceptions: new List<BusinessHoursExceptionsDto>
                {
                    new(new DateOnly(2025, 1, 1), "Closed"),
                    new(new DateOnly(2025, 12, 25), "Closed")
                }
            )
        };

        var addressAndCity = (address: "123 Main St", city: "New York");
        var establishment = new Establishment
        {
            Name = "Default Name",
            Auth0Id = auth0Id,
            Latitude = 0,
            Longitude = 0,
            Address = "",
            City = "",
            PriceCategory = PriceCategory.Moderate,
            BusinessHours = new BusinessHours
            {
                Monday = "",
                Tuesday = "",
                Wednesday = "",
                Thursday = "",
                Friday = "",
                Saturday = "",
                Sunday = "",
                BusinessHoursExceptions = new List<BusinessHoursException>()
            }
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        _mapboxServiceMock
            .Setup(s => s.GetAddressFromCoordinatesAsync(request.Lat, request.Long))
            .ReturnsAsync(addressAndCity);

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _establishmentService.UpdateEstablishmentLocationInfoAsync(auth0Id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(addressAndCity.address, result.Address);
        Assert.Equal(addressAndCity.city, result.City);
        Assert.Equal(request.BusinessHours.Monday, result.BusinessHours.Monday);
        Assert.Equal(request.BusinessHours.Tuesday, result.BusinessHours.Tuesday);
        Assert.Equal(request.BusinessHours.Wednesday, result.BusinessHours.Wednesday);
        Assert.Equal(request.BusinessHours.Thursday, result.BusinessHours.Thursday);
        Assert.Equal(request.BusinessHours.Friday, result.BusinessHours.Friday);
        Assert.Equal(request.BusinessHours.Saturday, result.BusinessHours.Saturday);
        Assert.Equal(request.BusinessHours.Sunday, result.BusinessHours.Sunday);
        Assert.Equal(
            request.BusinessHours.BusinessHoursExceptions.Count,
            result.BusinessHours.BusinessHoursExceptions.Count
        );

        foreach (var exception in request.BusinessHours.BusinessHoursExceptions)
        {
            Assert.Contains(result.BusinessHours.BusinessHoursExceptions, e =>
                e.Date == exception.Date && e.Status == exception.Status);
        }

        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Once);
        _mapboxServiceMock.Verify(s => s.GetAddressFromCoordinatesAsync(request.Lat, request.Long), Times.Once);
    }

    // UpdateEstablishmentPicturesAsync tests
    
    [Fact]
    public async Task UpdateEstablishmentPictureAsync_ValidRequest_ShouldUpdatePictureAndReturnUrl()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        const string pictureContext = "profile-picture";
        var pictureMock = new Mock<IFormFile>();
        var pictureUrl = "https://storage.example.com/establishment/auth0|123/profile-picture.jpg";

        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = auth0Id,
            ProfilePictureUrl = "",
            BackgroundPictureUrl = "",
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>()
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true))
            .ReturnsAsync(establishment);

        _storageServiceMock
            .Setup(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(pictureUrl);

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _establishmentService.UpdateEstablishmentPictureAsync(auth0Id, pictureContext, pictureMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pictureUrl, result);
        Assert.Equal(pictureUrl, establishment.ProfilePictureUrl);
        Assert.Equal("", establishment.BackgroundPictureUrl);

        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Once);
        _storageServiceMock.Verify(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEstablishmentPictureAsync_InvalidPictureContext_ShouldThrowArgumentException()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        const string pictureContext = "invalid-context";
        var pictureMock = new Mock<IFormFile>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _establishmentService.UpdateEstablishmentPictureAsync(auth0Id, pictureContext, pictureMock.Object));

        Assert.Equal("Invalid image context (Parameter 'pictureContext')", exception.Message);
    }
    
    // UpdateRatingStatisticAsync tests
    
    [Fact]
    public async Task UpdateRatingStatisticsAsync_ValidRequest_ShouldUpdateRatingStatistics()
    {
        // Arrange
        const int id = 1;
        const int rating = 4;
        const bool increment = true;
        var establishment = new Establishment
        {
            Auth0Id = "auth0|123",
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            RatingStatistic = new RatingStatistic
            {
                Id = id,
                OneStar = 1,
                TwoStars = 2,
                ThreeStars = 3,
                FourStars = 4,
                FiveStars = 5
            }
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
            .Returns(Task.CompletedTask);

        // Act
        await _establishmentService.UpdateRatingStatisticsAsync(id, rating);

        // Assert
        Assert.Equal(5, establishment.RatingStatistic.FourStars);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Once);
    }

    [Fact]
    public async Task UpdateRatingStatisticsAsync_DecrementRating_ShouldUpdateRatingStatistics()
    {
        // Arrange
        const int id = 1;
        const int rating = 4;
        const bool increment = false;
        var establishment = new Establishment
        {
            Auth0Id = "auth0|123",
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            RatingStatistic = new RatingStatistic
            {
                Id = id,
                OneStar = 1,
                TwoStars = 2,
                ThreeStars = 3,
                FourStars = 4,
                FiveStars = 5
            }
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
            .Returns(Task.CompletedTask);

        // Act
        await _establishmentService.UpdateRatingStatisticsAsync(id, rating, increment);

        // Assert
        Assert.Equal(3, establishment.RatingStatistic.FourStars);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Once);
    }

    [Fact]
    public async Task UpdateRatingStatisticsAsync_RatingStatisticNotFound_ShouldThrowException()
    {
        // Arrange
        const int id = 1;
        const int rating = 4;
        var increment = true;
        var establishment = new Establishment
        {
            Auth0Id = "auth0|123",
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            RatingStatistic = null 
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _establishmentService.UpdateRatingStatisticsAsync(id, rating, increment));

        Assert.Equal("RatingStatistic not found for the establishment.", exception.Message);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRatingStatisticsAsync_InvalidRating_ShouldThrowArgumentException()
    {
        // Arrange
        const int id = 1;
        const int rating = 6; 
        const bool increment = true;
        var establishment = new Establishment
        {
            Auth0Id = "auth0|123",
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            RatingStatistic = new RatingStatistic
            {
                Id = id,
                OneStar = 1,
                TwoStars = 2,
                ThreeStars = 3,
                FourStars = 4,
                FiveStars = 5
            }
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _establishmentService.UpdateRatingStatisticsAsync(id, rating));

        Assert.Equal("Invalid rating value (Parameter 'rating')", exception.Message);
        Assert.Equal("rating", exception.ParamName);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Never);
    }

    
    // UpdateEstablishmentRegisterRequestAsync tests

    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_ValidRequest_UpdatesRequestStatus()
    {
        // Arrange
        const string auth0Id = "validAuth0Id";
        const RequestStatus requestStatus = RequestStatus.Accepted;
        
        var establishment = new Establishment
        {
            Auth0Id = auth0Id,
            Name = "Default Name",
            City = "Default City",
            Address = "Default Address",
            Latitude = 0.0,
            Longitude = 0.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending
        };
        
        _establishmentRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>())).ReturnsAsync(establishment);

        // Act
        await _establishmentService.UpdateEstablishmentRegisterRequestAsync(auth0Id, requestStatus);

        // Assert
        Assert.Equal(requestStatus, establishment.RequestStatus);
        _establishmentRepositoryMock.Verify(repo => repo.UpdateAsync(establishment), Times.Once);
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
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _establishmentService.DeleteEstablishmentAsync(auth0Id));
    }
    
    // AddMenuAsync tests
    
    [Fact]
    public async Task AddMenuAsync_ValidRequest_ShouldAddMenuToEstablishmentAndReturnMenuDto()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var menuRequest = new MenuCreateRequest
        {
            Name = "Test Menu",
            MenuItems = new List<MenuItemCreateRequest>
            {
                new() { Name = "Pizza", Description = "Delicious pizza", Price = 9.99 },
                new() { Name = "Pasta", Description = "Tasty pasta", Price = 12.99 }
            }
        };

        var establishment = new Establishment
        {
            Auth0Id = auth0Id,
            Id = 1,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            Menus = new List<Menu>()
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _establishmentService.AddMenuAsync(auth0Id, menuRequest);

        // Assert
        Assert.NotNull(result); 
        Assert.Equal(menuRequest.Name, result.Name); 
        Assert.Equal(menuRequest.MenuItems.Count, result.MenuItems.Count); 
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Once);
    }

    [Fact]
    public async Task AddMenuAsync_EstablishmentNotFound_ShouldThrowException()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var menuRequest = new MenuCreateRequest
        {
            Name = "Test Menu",
            MenuItems = new List<MenuItemCreateRequest>
            {
                new() { Name = "Pizza", Description = "Delicious pizza", Price = 9.99 }
            }
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ThrowsAsync(new EntityNotFoundException("Establishment not found"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _establishmentService.AddMenuAsync(auth0Id, menuRequest));

        Assert.Equal("The Establishment not found was not found", exception.Message);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Never);
    }

    // UpdateMenuAsync tests
    
    [Fact]
    public async Task UpdateMenuAsync_ValidRequest_ShouldUpdateMenuAndReturnMenuDto()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var menuUpdateRequest = new MenuUpdateRequest
        {
            Id = 1,
            Name = "Test Menu",
            MenuItems = new List<MenuItemUpdateRequest>
            {
                new() { Id = 1, Name = "Updated Pizza", Description = "Updated Description", Price = 10.99 },
                new() { Id = null, Name = "New Item", Description = "New Item Description", Price = 8.99 }
            }
        };

        var existingMenuItem = new MenuItem
        {
            Id = 1,
            Name = "Pizza",
            Description = "Delicious pizza",
            Price = 9.99
        };

        var menu = new Menu
        {
            Id = 1,
            Name = "Test Menu",
            MenuItems = new List<MenuItem> { existingMenuItem }
        };

        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            Menus = new List<Menu> { menu }
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _establishmentService.UpdateMenuAsync(auth0Id, menuUpdateRequest);

        // Assert
        Assert.NotNull(result); 
        Assert.Equal(menuUpdateRequest.MenuItems.Count, result.MenuItems.Count);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMenuAsync_MenuNotFound_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var menuUpdateRequest = new MenuUpdateRequest
        {
            Id = 1,
            Name = "Test Menu",
            MenuItems = new List<MenuItemUpdateRequest>
            {
                new() { Id = 1, Name = "Updated Pizza", Description = "Updated Description", Price = 10.99 }
            }
        };

        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            Menus = new List<Menu>() 
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _establishmentService.UpdateMenuAsync(auth0Id, menuUpdateRequest));
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Never);
    }

    [Fact]
    public async Task UpdateMenuAsync_MenuItemNotFound_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var menuUpdateRequest = new MenuUpdateRequest
        {
            Id = 1,
            Name = "Test Menu",
            MenuItems = new List<MenuItemUpdateRequest>
            {
                new() { Id = 99, Name = "Non-existent Item", Description = "Item doesn't exist", Price = 10.99 }
            }
        };

        var existingMenuItem = new MenuItem
        {
            Id = 1,
            Name = "Pizza",
            Description = "Delicious pizza",
            Price = 9.99
        };

        var menu = new Menu
        {
            Id = 1,
            Name = "Test Menu",
            MenuItems = new List<MenuItem> { existingMenuItem }
        };

        var establishment = new Establishment
        {
            Id = 1,
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            Menus = new List<Menu> { menu }
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _establishmentService.UpdateMenuAsync(auth0Id, menuUpdateRequest));
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Never);
    }

    // UpdateMenuItemPictureAsync tests
    
    [Fact]
    public async Task UpdateMenuItemPictureAsync_ValidRequest_ShouldUpdatePictureAndReturnUrl()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        const int menuItemId = 1;
        var picture = new Mock<IFormFile>();
        picture.Setup(p => p.ContentType).Returns("image/jpeg");

        var establishment = new Establishment
        {
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            Menus = new List<Menu>
            {
                new()
                {
                    Id = 1,
                    Name = "Test Menu",
                    MenuItems = new List<MenuItem>
                    {
                        new()
                        {
                            Id = menuItemId,
                            Name = "Pizza",
                            Description = "Delicious pizza",
                            Price = 9.99,
                            PictureUrl = null,
                            Menu = new Menu { Id = 1, Name = "Test Menu"}
                        }
                    }
                }
            }
        };

        var pictureUrl = "https://storage.url/menu/1/1/picture.jpg";

        _storageServiceMock
            .Setup(s => s.CheckPictureType(It.IsAny<string>()))
            .Verifiable();
        
        _storageServiceMock
            .Setup(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), "Establishment", auth0Id, It.IsAny<string>()))
            .ReturnsAsync(pictureUrl);

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _establishmentService.UpdateMenuItemPictureAsync(auth0Id, menuItemId, picture.Object);

        // Assert
        Assert.Equal(pictureUrl, result);
        _storageServiceMock.Verify(s => s.CheckPictureType(It.IsAny<string>()), Times.Once); 
        _storageServiceMock.Verify(s => s.UploadPictureAsync(It.IsAny<IFormFile>(), "Establishment", auth0Id, It.IsAny<string>()), Times.Once); 
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Once); 
    }

    [Fact]
    public async Task UpdateMenuItemPictureAsync_MenuItemNotFound_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        const int menuItemId = 99;
        var picture = new Mock<IFormFile>();
        picture.Setup(p => p.ContentType).Returns("image/jpeg");

        var establishment = new Establishment
        {
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            Menus = new List<Menu>
            {
                new()
                {
                    Id = 1,
                    Name = "Test Menu",
                    MenuItems = new List<MenuItem>()
                }
            }
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _establishmentService.UpdateMenuItemPictureAsync(auth0Id, menuItemId, picture.Object));

        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Never);
    }
    
    // DeleteMenuItemAsync tests
    
    [Fact]
    public async Task DeleteMenuAsync_ValidMenuId_ShouldDeleteMenu()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        const int menuId = 1;

        var establishment = new Establishment
        {
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            Menus = new List<Menu>
            {
                new() { Id = menuId, Name = "Test Menu" }
            }
        };

        _storageServiceMock
            .Setup(s => s.DeleteAsync("Establishment", auth0Id, $"menu/{menuId}"))
            .Returns(Task.CompletedTask);

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
            .Returns(Task.CompletedTask);

        // Act
        await _establishmentService.DeleteMenuAsync(auth0Id, menuId);

        // Assert
        Assert.DoesNotContain(establishment.Menus, m => m.Id == menuId); 
        _storageServiceMock.Verify(s => s.DeleteAsync("Establishment", auth0Id, $"menu/{menuId}"), Times.Once);
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(establishment), Times.Once); 
    }

    [Fact]
    public async Task DeleteMenuAsync_MenuNotFound_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        const int menuId = 99;

        var establishment = new Establishment
        {
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            Menus = new List<Menu>()
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _establishmentService.DeleteMenuAsync(auth0Id, menuId));
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Never);
    }

    [Fact]
    public async Task DeleteMenuItemAsync_ValidMenuItemId_ShouldDeleteMenuItem()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        const int menuItemId = 1;

        var establishment = new Establishment
        {
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            Menus = new List<Menu>
            {
                new()
                {
                    Id = 1,
                    Name = "Test Menu",
                    MenuItems = new List<MenuItem>
                    {
                        new() { Id = menuItemId, Name = "Pizza", Description = "Delicious pizza", Price = 9.99 }
                    }
                }
            }
        };

        _storageServiceMock
            .Setup(s => s.DeleteAsync("Establishment", auth0Id, $"menu/1/{menuItemId}"))
            .Returns(Task.CompletedTask);

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        _establishmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
            .Returns(Task.CompletedTask);

        // Act
        await _establishmentService.DeleteMenuItemAsync(auth0Id, menuItemId);

        // Assert
        var menu = establishment.Menus.First();
        Assert.DoesNotContain(menu.MenuItems, mi => mi.Id == menuItemId);
        _storageServiceMock.Verify(s => s.DeleteAsync("Establishment", auth0Id, $"menu/1/{menuItemId}"), Times.Once); 
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(establishment), Times.Once); 
    }

    [Fact]
    public async Task DeleteMenuItemAsync_MenuItemNotFound_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        const int menuItemId = 99;

        var establishment = new Establishment
        {
            Auth0Id = auth0Id,
            Name = "test-name",
            Description = "test-description",
            City = "test-city",
            Address = "test-address",
            Latitude = 1.0,
            Longitude = 1.0,
            PriceCategory = PriceCategory.Moderate,
            RequestStatus = RequestStatus.Pending,
            Categories = new List<Category>(),
            Tags = new List<Tag>(),
            Menus = new List<Menu>
            {
                new() { Id = 1, Name = "Test Name",MenuItems = new List<MenuItem>() } 
            }
        };

        _establishmentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Establishment, bool>>>(), true, It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>[]>()))
            .ReturnsAsync(establishment);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _establishmentService.DeleteMenuItemAsync(auth0Id, menuItemId));
        _establishmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Establishment>()), Times.Never); // Ensure UpdateAsync is not called
    }
}