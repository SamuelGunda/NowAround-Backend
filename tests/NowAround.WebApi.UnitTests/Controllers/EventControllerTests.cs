using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NowAround.Application.Dtos;
using NowAround.Application.Requests;
using NowAround.Application.Services;

namespace NowAround.WebApi.Controllers;

public class EventControllerTests
{
    private readonly Mock<IEventService> _mockEventService;
    private readonly EventController _controller;

    public EventControllerTests()
    {
        _mockEventService = new Mock<IEventService>();
        _controller = new EventController(_mockEventService.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, "auth0Id123")
        ]));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task CreateEventAsync_ReturnsCreatedResult_WhenEventCreated()
    {
        // Arrange
        var eventCreateRequest = new EventCreateRequest
        {
            Title = "Sample Event",
            Body = "Sample Event Description",
            Price = "10",
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            EventCategory = "Other",
            EventPriceCategory = "*",
            Address = "Sample Address, 420",
            City = "Sample City"
        };

        var createdEventDto = new EventDto(
            1,
            "auth0Id123",
            "Sample Event",
            "Sample Event Description",
            "10",
            "*",
            "Sample City",
            "Sample Address, 420",
            10,
            10,
            null,
            null,
            DateTime.Now,
            DateTime.Now.AddHours(1),
            "Other",
            DateTime.Now,
            []
        );
            
            
        _mockEventService.Setup(service => service.CreateEventAsync(It.IsAny<string>(), It.IsAny<EventCreateRequest>()))
            .ReturnsAsync(createdEventDto);

        // Act
        var result = await _controller.CreateEventAsync(eventCreateRequest);

        // Assert
        var actionResult = Assert.IsType<CreatedResult>(result);
        var returnValue = actionResult.Value as EventDto;
        Assert.NotNull(returnValue);
        _mockEventService.Verify(service => service.CreateEventAsync(It.IsAny<string>(), It.IsAny<EventCreateRequest>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateEventAsync_ThrowsArgumentException_WhenAuth0IdMissing()
    {
        // Arrange
        var invalidUser = new ClaimsPrincipal(new ClaimsIdentity());
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = invalidUser }
        };

        var eventCreateRequest = new EventCreateRequest
        {
            Title = "Sample Event",
            Body = "Sample Event Description",
            Price = "10",
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            EventCategory = "Other",
            EventPriceCategory = "*",
            Address = "Sample Address, 420",
            City = "Sample City"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.CreateEventAsync(eventCreateRequest));
        Assert.Equal("Auth0Id not found", exception.Message);
    }

    [Fact]
    public async Task ReactToEventAsync_ReturnsOk_WhenEventReactedTo()
    {
        // Arrange
        var eventId = 1;
        _mockEventService.Setup(service => service.ReactToEventAsync(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ReactToEventAsync(eventId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = actionResult.Value as dynamic;
        Assert.NotNull(returnValue);
        Assert.Equal("Reacted to event successfully", returnValue.message);
        _mockEventService.Verify(service => service.ReactToEventAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task DeleteEventAsync_ReturnsNoContent_WhenEventDeleted()
    {
        // Arrange
        var eventId = 1;
        _mockEventService.Setup(service => service.DeleteEventAsync(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteEventAsync(eventId);

        // Assert
        var actionResult = Assert.IsType<NoContentResult>(result);
        _mockEventService.Verify(service => service.DeleteEventAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
    }

    
}