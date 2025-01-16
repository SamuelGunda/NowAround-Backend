using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NowAround.Application.Dtos;
using NowAround.Application.Requests;
using NowAround.Application.Responses;
using NowAround.Application.Services;
using NowAround.Domain.Enum;

namespace NowAround.WebApi.Controllers;

public class EstablishmentControllerTests
{
    private readonly Mock<IEstablishmentService> _mockEstablishmentService;
    private readonly EstablishmentController _controller;

    public EstablishmentControllerTests()
    {
        _mockEstablishmentService = new Mock<IEstablishmentService>();
        _controller = new EstablishmentController(_mockEstablishmentService.Object);
    }

    private void SetUpAuth0Id(string auth0Id)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, auth0Id)
        };
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    // RegisterEstablishmentAsync tests
    [Fact]
    public async Task RegisterEstablishmentAsync_ReturnsCreatedResponse()
    {
        // Arrange
        var request = new EstablishmentRegisterRequest
        {
            EstablishmentInfo = new EstablishmentInfo
            {
                Name = "Establishment Name",
                Address = "Establishment Address",
                PostalCode = "123456",
                City = "City",
                PriceCategory = 0,
                Category= new List<string> { "Category1", "Category2" },
                Tags = new List<string> { "Tag1", "Tag2" }
            },
            EstablishmentOwnerInfo = new EstablishmentOwnerInfo
            {
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "Test@123.sk"
            }
        };
        _mockEstablishmentService.Setup(service => service.RegisterEstablishmentAsync(It.IsAny<EstablishmentRegisterRequest>()))
                                 .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RegisterEstablishmentAsync(request);

        // Assert
        var actionResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal("", actionResult.Location);
    }

    // GetEstablishmentProfileInfoByAuth0IdAsync tests
    
    [Fact]
    public async Task GetEstablishmentProfileInfoByAuth0IdAsync_ReturnsOkResult()
    {
        // Arrange
        const string auth0Id = "auth0Id123";
        
        var establishment = new EstablishmentProfileResponse(
            "auth0Id123",
            new GenericInfo(
                "Vaše Demo",
                "https://nowaroundimagestorage.blob.core.windows.net/establishment/auth0-6719413321911e8a1a607b29/profile-picture.png",
                "https://nowaroundimagestorage.blob.core.windows.net/establishment/auth0-6719413321911e8a1a607b29/background-picture.jpeg",
                "Toto je demo",
                "Expensive",
                ["WIFI_AVAILABLE", "TAKEOUT_AVAILABLE", "BAR_SERVICE"],
                ["GYM", "CINEMA"],
                []
            ),
            new LocationInfo(
                "Továrenská 320/4, 040 01",
                "Košice",
                21.255554151445494,
                48.72652910698301,
                new BusinessHoursDto(
                    "Closed - Closed",
                    "08:00 - 17:00",
                    "Closed - Closed",
                    "Closed - Closed",
                    "08:00 - 17:00",
                    "Closed - Closed",
                    "08:00 - 17:00",
                    new List<BusinessHoursExceptionsDto>
                    {
                        new(It.IsAny<DateOnly>(), "asd"),
                        new(It.IsAny<DateOnly>(), "asv")
                    }
                )
            ),
            [
                new PostDto(1, null, "sad", "asdasd", null, DateTime.Parse("2025-01-13T11:02:41.5195625"),
                    [])
            ],
            [
                new MenuDto(1, "sa", new List<MenuItemDto>
                {
                    new(1, "afs",
                        "https://nowaroundimagestorage.blob.core.windows.net/establishment/auth0-6719413321911e8a1a607b29/menu/1/1.png",
                        "asf", 45)
                }),

                new MenuDto(2, "Drinks", new List<MenuItemDto>
                {
                    new(2, "Pifko", null, "Je dobre", 0.5)
                })
            ],
            [
                new EventDto(12, null, "asd", "asd", "asd", "*", "Košice", "Továrenská 320/4, 040 01", 48.726647,
                    21.255318, "11 - 20", null, DateTime.Parse("2025-01-17T23:27:00"),
                    DateTime.Parse("2025-01-30T23:26:00"), "Food", DateTime.Parse("2025-01-15T22:23:12.0549535"),
                    [])
            ],
            new RatingStatisticResponse(
                1, 0, 0, 1, 1,
                new List<ReviewWithAuthIdsResponse>
                {
                    new("google-oauth2|103531829329531613345", null, "Rastislav Pačut", "Fajne demo", 5, DateTime.Parse("2024-12-16T13:51:25")),
                    new("google-oauth2|107525997574903165087", null, "Michaela Majorošová", "Pohode", 4, DateTime.Parse("2024-12-16T13:51:27")),
                    new("google-oauth2|115080570169134798005", null, "Samuel Gunda", "Som kysli", 1, DateTime.Parse("2024-12-16T13:51:27"))
                }
            )
        );
        
        _mockEstablishmentService.Setup(service => service.GetEstablishmentProfileByAuth0IdAsync(auth0Id))
                                 .ReturnsAsync(establishment);
        SetUpAuth0Id(auth0Id);

        // Act
        var result = await _controller.GetEstablishmentProfileInfoByAuth0IdAsync(auth0Id);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(establishment, actionResult.Value);
    }
    
    // GetPendingEstablishmentsAsync tests
    
    [Fact]
    public async Task GetPendingEstablishmentsAsync_ReturnsNoContent_WhenNoEstablishments()
    {
        // Arrange
        _mockEstablishmentService
            .Setup(service => service.GetPendingEstablishmentsAsync())
            .ReturnsAsync([]);

        // Act
        var result = await _controller.GetPendingEstablishmentsAsync();

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetPendingEstablishmentsAsync_ReturnsOk_WhenEstablishmentsFound()
    {
        // Arrange
        var establishments = new List<PendingEstablishmentResponse>
        {
            new(
                "auth0Id123",
                "Establishment Name",
                "test"
            )
        };

        _mockEstablishmentService
            .Setup(service => service.GetPendingEstablishmentsAsync())
            .ReturnsAsync(establishments);

        // Act
        var result = await _controller.GetPendingEstablishmentsAsync();

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = actionResult.Value as List<PendingEstablishmentResponse>;
        Assert.NotEmpty(returnValue);
        Assert.Equal(establishments.Count, returnValue.Count);
    }
    
    // GetEstablishmentsWithFilterAsync tests
    
    [Fact]
    public async Task GetEstablishmentsWithFilterAsync_ReturnsNoContent_WhenNoEstablishmentsFound()
    {
        // Arrange
        _mockEstablishmentService
            .Setup(service => service.GetEstablishmentsWithFilterAsync(It.IsAny<SearchValues>(), 0))
            .ReturnsAsync([]);

        // Act
        var result = await _controller.GetEstablishmentsWithFilterAsync(
            northWestLat: 10, northWestLong: 10,
            southEastLat: 20, southEastLong: 20,
            name: "Test", priceCategory: null, 
            categoryName: null, tagNames: null, 
            page: 0
        );

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetEstablishmentsWithFilterAsync_ReturnsOk_WhenEstablishmentsFound()
    {
        // Arrange
        var establishments = new List<EstablishmentMarkerResponse> { 
            new(
                "auth0Id123",
                "Establishment Name",
                "test",
                "Moderate",
                ["test"],
                0,
                0
                )
        };
        _mockEstablishmentService
            .Setup(service => service.GetEstablishmentsWithFilterAsync(It.IsAny<SearchValues>(), 0))
            .ReturnsAsync(establishments);

        // Act
        var result = await _controller.GetEstablishmentsWithFilterAsync(
            northWestLat: 10, northWestLong: 10,
            southEastLat: 20, southEastLong: 20,
            name: "Test", priceCategory: null, 
            categoryName: null, tagNames: null, 
            page: 0
        );

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    // UpdateEstablishmentGenericInfoAsync tests
    
    [Fact]
    public async Task UpdateEstablishmentGenericInfoAsync_ReturnsOk_WhenUpdateSucceeds()
    {
        // Arrange
        var establishmentUpdateRequest = new EstablishmentGenericInfoUpdateRequest { Name = "New Name", PriceCategory = 1, Categories = new List<string> { "Category1" }, Description = "", Tags = new List<string> { "Tag1" } };
        const string auth0Id = "auth0|123";
        var updatedInfo = new GenericInfo("New Name", null, null, null, "Affordable", null, null, null);

        _mockEstablishmentService
            .Setup(service => service.UpdateEstablishmentGenericInfoAsync(auth0Id, establishmentUpdateRequest))
            .ReturnsAsync(updatedInfo);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([
                    new Claim(ClaimTypes.NameIdentifier, auth0Id)
                ]))
            }
        };

        // Act
        var result = await _controller.UpdateEstablishmentGenericInfoAsync(establishmentUpdateRequest);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = actionResult.Value as GenericInfo;
        Assert.NotNull(returnValue);
        Assert.Equal("New Name", returnValue.Name);
    }

    [Fact]
    public async Task UpdateEstablishmentGenericInfoAsync_ThrowsArgumentException_WhenAuth0IdNotFound()
    {
        // Arrange
        var establishmentUpdateRequest = new EstablishmentGenericInfoUpdateRequest { Name = "New Name", PriceCategory = 1, Categories = new List<string> { "Category1" }, Description = "", Tags = new List<string> { "Tag1" } };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _controller.UpdateEstablishmentGenericInfoAsync(establishmentUpdateRequest)
        );
        Assert.Equal("Auth0Id not found", exception.Message);
    }
    
    // UpdateEstablishmentLocationInfoAsync tests

    [Fact]
    public async Task UpdateEstablishmentLocationInfoAsync_ReturnsOk_WhenUpdateSucceeds()
    {
        // Arrange
        var establishmentUpdateRequest = new EstablishmentLocationInfoUpdateRequest
        {
            Lat = 1, Long = 1,
            BusinessHours = new BusinessHoursDto("", "", "", "", "", "", "", new List<BusinessHoursExceptionsDto>())
        };
        
        var auth0Id = "auth0|123";
        var updatedInfo = new LocationInfo("Address", "City", 1, 1, new BusinessHoursDto("", "", "", "", "", "", "", new List<BusinessHoursExceptionsDto>()));
        
        _mockEstablishmentService
            .Setup(service => service.UpdateEstablishmentLocationInfoAsync(auth0Id, establishmentUpdateRequest))
            .ReturnsAsync(updatedInfo);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([
                    new Claim(ClaimTypes.NameIdentifier, auth0Id)
                ]))
            }
        };
        
        // Act
        var result = await _controller.UpdateEstablishmentLocationInfoAsync(establishmentUpdateRequest);
        
        // Assert
        Assert.IsType<OkObjectResult>(result);
        
    }
    // UpdateEstablishmentPictureAsync tests
    
    [Fact]
    public async Task UpdateEstablishmentPictureAsync_ReturnsOkResult()
    {
        // Arrange
        const string auth0Id = "auth0Id123";
        const string pictureContext = "profile";
        var mockFile = new Mock<IFormFile>();
        const string pictureUrl = "http://example.com/picture.jpg";
        _mockEstablishmentService.Setup(service => service.UpdateEstablishmentPictureAsync(auth0Id, pictureContext, mockFile.Object))
                                 .ReturnsAsync(pictureUrl);
        SetUpAuth0Id(auth0Id);

        // Act
        var result = await _controller.UpdateEstablishmentPictureAsync(pictureContext, mockFile.Object);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
    
    // UpdateEstablishmentRegisterRequestAsync tests
    
    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_ReturnsNoContent_WhenStatusIsAccepted()
    {
        // Arrange
        const string auth0Id = "auth0Id123";
        const string action = "accept";
        _mockEstablishmentService.Setup(service => service.UpdateEstablishmentRegisterRequestAsync(auth0Id, RequestStatus.Accepted))
                                 .Returns(Task.CompletedTask);
        
        // Act
        var result = await _controller.UpdateEstablishmentRegisterRequestAsync(auth0Id, action);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }
    
    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_ReturnsNoContent_WhenStatusIsRejected()
    {
        // Arrange
        const string auth0Id = "auth0Id123";
        const string action = "reject";
        _mockEstablishmentService.Setup(service => service.UpdateEstablishmentRegisterRequestAsync(auth0Id, RequestStatus.Rejected))
                                 .Returns(Task.CompletedTask);
        
        // Act
        var result = await _controller.UpdateEstablishmentRegisterRequestAsync(auth0Id, action);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }
    
    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        // Arrange
        var auth0Id = "auth0Id123";
        var action = "invalid";
        
        // Act
        var result = await _controller.UpdateEstablishmentRegisterRequestAsync(auth0Id, action);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    // DeleteEstablishmentAsync tests
    
    [Fact]
    public async Task DeleteEstablishmentAsync_ReturnsNoContent()
    {
        // Arrange
        const string auth0Id = "auth0Id123";
        _mockEstablishmentService.Setup(service => service.DeleteEstablishmentAsync(auth0Id))
                                 .Returns(Task.CompletedTask);
        SetUpAuth0Id(auth0Id);
        
        // Act
        var result = await _controller.DeleteEstablishmentAsync();
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }
    
    // AddMenuAsync tests
    
    [Fact]
    public async Task AddMenuAsync_ReturnsCreatedResponse()
    {
        // Arrange
        const string auth0Id = "auth0Id123";
        var menu = new MenuCreateRequest { Name="Menu Name" };
        var menuDto = new MenuDto(1, "Menu Name", new List<MenuItemDto>());
        _mockEstablishmentService.Setup(service => service.AddMenuAsync(auth0Id, menu))
                                 .ReturnsAsync(menuDto);
        SetUpAuth0Id(auth0Id);
        
        // Act
        var result = await _controller.AddMenuAsync(menu);
        
        // Assert
        Assert.IsType<CreatedResult>(result);
    }
    
    // UpdateMenuAsync tests
    
    [Fact]
    public async Task UpdateMenuAsync_ReturnsOkResult()
    {
        // Arrange
        const string auth0Id = "auth0Id123";
        var menu = new MenuUpdateRequest { Id = 1, Name = "Menu Name" };
        var menuDto = new MenuDto(1, "Menu Name", new List<MenuItemDto>());
        _mockEstablishmentService.Setup(service => service.UpdateMenuAsync(auth0Id, menu))
                                 .ReturnsAsync(menuDto);
        SetUpAuth0Id(auth0Id);
        
        // Act
        var result = await _controller.UpdateMenuAsync(menu);
        
        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    // UpdateMenuItemPictureAsync tests
    [Fact]
    public async Task UpdateMenuItemPictureAsync_ReturnsOkResult()
    {
        // Arrange
        const string auth0Id = "auth0Id123";
        const int menuItemId = 1;
        var mockFile = new Mock<IFormFile>();
        const string pictureUrl = "http://example.com/menuitem.jpg";
        _mockEstablishmentService.Setup(service => service.UpdateMenuItemPictureAsync(auth0Id, menuItemId, mockFile.Object))
                                 .ReturnsAsync(pictureUrl);
        SetUpAuth0Id(auth0Id);

        // Act
        var result = await _controller.UpdateMenuItemPictureAsync(menuItemId, mockFile.Object);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    // DeleteMenuAsync tests
    [Fact]
    public async Task DeleteMenuAsync_ReturnsNoContent()
    {
        // Arrange
        const string auth0Id = "auth0Id123";
        const int menuId = 1;
        _mockEstablishmentService.Setup(service => service.DeleteMenuAsync(auth0Id, menuId))
            .Returns(Task.CompletedTask);
        SetUpAuth0Id(auth0Id);

        // Act
        var result = await _controller.DeleteMenuAsync(menuId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
    
    // DeleteMenuItemAsync tests

    [Fact]
    public async Task DeleteMenuItemAsync_ReturnsNoContent()
    {
        // Arrange
        const string auth0Id = "auth0Id|123";
        const int menuItemId = 1;
        
        _mockEstablishmentService.Setup(service => service.DeleteMenuItemAsync(auth0Id, menuItemId))
                                 .Returns(Task.CompletedTask);
        
        SetUpAuth0Id(auth0Id);
        
        // Act
        var result = await _controller.DeleteMenuItemAsync(menuItemId);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
