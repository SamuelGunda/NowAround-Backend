﻿using System.Linq.Expressions;
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
        
        var establishmentProfile = new EstablishmentProfileResponse(
            establishment.Auth0Id,
            new GenericInfo(
                establishment.Name,
                "Default",
                "Default",
                establishment.Description,
                "Moderate",
                [],
                [],
                []
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
            [],
            [],
            [],
            new RatingStatisticResponse(
                0, 0, 0, 0, 0,
                new List<ReviewWithAuthIdsResponse>()
            )
        );
        
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
    
    // GetEstablishmentWithFilterAsync

    [Fact]
    public async Task GetEstablishmentsWithFilterAsync_ShouldReturnEstablishments_WhenSearchValuesAreValid()
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
        var page = 1;
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
    public void GetEstablishmentsWithFilterAsync_ShouldThrowException_WhenSearchValuesAreInvalid()
    {
        // Arrange
        var searchValues = new SearchValues
        {
            Name = null, // Invalid SearchValues: No Name, PriceCategory, or CategoryName set
            PriceCategory = null,
            CategoryName = null,
            TagNames = null,
            MapBounds = new MapBounds { NwLat = 0, NwLong = 0, SeLat = 0, SeLong = 0 } // Invalid MapBounds
        };
        var page = 1;

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidSearchActionException>(
            async () => await _establishmentService.GetEstablishmentsWithFilterAsync(searchValues, page));

        Assert.Equal("Invalid search values", exception.Result.Message);
    }

    [Fact]
    public void GetEstablishmentsWithFilterAsync_ShouldThrowException_WhenPageIsInvalid()
    {
        // Arrange
        var searchValues = new SearchValues
        {
            Name = "Test Establishment",
            PriceCategory = 1,
            CategoryName = "Restaurant",
            TagNames = new List<string> { "Pet_Friendly" },
            MapBounds = new MapBounds { NwLat = 10, NwLong = 20, SeLat = 5, SeLong = 25 }
        };
        var page = -1;

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidSearchActionException>(
            async () => await _establishmentService.GetEstablishmentsWithFilterAsync(searchValues, page));

        Assert.Equal("Page must be greater than 0", exception.Result.Message);
    }

    [Fact]
    public async Task GetEstablishmentsWithFilterAsync_ShouldReturnEmptyList_WhenNoResultsFromRepository()
    {
        // Arrange
        var searchValues = new SearchValues
        {
            Name = "Test Establishment",
            PriceCategory = 1,
            CategoryName = "Restaurant",
            TagNames = new List<string> { "Pet_Friendly" },
            MapBounds = new MapBounds { NwLat = 10, NwLong = 20, SeLat = 5, SeLong = 25 }
        };
        var page = 1;
        
        _establishmentRepositoryMock.Setup(r => r.GetRangeWithFilterAsync(It.IsAny<Func<IQueryable<Establishment>, IQueryable<Establishment>>>(), page))
            .ReturnsAsync(new List<Establishment>()); // Return empty list

        // Act
        var result = await _establishmentService.GetEstablishmentsWithFilterAsync(searchValues, page);

        // Assert
        Assert.Empty(result); // Ensure the result is empty
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
}