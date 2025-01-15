using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Application.Interfaces;
using NowAround.Application.Requests;
using NowAround.Application.Services;
using NowAround.Domain.Enum;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;

namespace NowAround.Application.UnitTests.Services;

public class EventServiceTests
{
    private readonly Mock<IEstablishmentService> _establishmentServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IMapboxService> _mapboxServiceMock;
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<ILogger<Event>> _loggerMock;
    private readonly EventService _eventService;
    
    public EventServiceTests()
    {
        _establishmentServiceMock = new Mock<IEstablishmentService>();
        _userServiceMock = new Mock<IUserService>();
        _mapboxServiceMock = new Mock<IMapboxService>();
        _storageServiceMock = new Mock<IStorageService>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _loggerMock = new Mock<ILogger<Event>>();

        _eventService = new EventService(
            _establishmentServiceMock.Object,
            _userServiceMock.Object,
            _mapboxServiceMock.Object,
            _storageServiceMock.Object,
            _eventRepositoryMock.Object,
            _loggerMock.Object);
    }
    
    [Fact]
    public async Task CreateEventAsync_ShouldCreateEvent_WhenValidRequest()
    {
        // Arrange
        const string auth0Id = "test-auth0-id";
        var eventCreateRequest = new EventCreateRequest
        {
            Title = "Test Event",
            Body = "Test Body",
            Price = "Free",
            EventPriceCategory = "Free",
            Address = "123 Test St, 12345",
            City = "Test City",
            MaxParticipants = "100",
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow.AddHours(2),
            EventCategory = "Music"
        };

        var coordinates = (lat: 1.0, lng: 1.0);
        
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
        
        _establishmentServiceMock.Setup(es => es.GetEstablishmentByAuth0IdAsync(auth0Id, true)).ReturnsAsync(establishment);
        _mapboxServiceMock.Setup(ms => ms.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(coordinates);
        _eventRepositoryMock.Setup(er => er.CreateAsync(It.IsAny<Event>())).ReturnsAsync(1);

        // Act
        var result = await _eventService.CreateEventAsync(auth0Id, eventCreateRequest);

        // Assert
        _eventRepositoryMock.Verify(er => er.CreateAsync(It.IsAny<Event>()), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(eventCreateRequest.Title, result.Title);
        Assert.Equal(coordinates.lat, result.Latitude);
        Assert.Equal(coordinates.lng, result.Longitude);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldThrowArgumentException_WhenAddressIsInvalid()
    {
        // Arrange
        const string auth0Id = "auth0|123";
        var eventCreateRequest = new EventCreateRequest
        {
            Title = "Test Event",
            Body = "Test Body",
            Price = "Free",
            EventPriceCategory = "Free",
            Address = "123 Test St", // Invalid address format
            City = "Test City",
            MaxParticipants = "100",
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow.AddHours(2),
            EventCategory = "Music"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await _eventService.CreateEventAsync(auth0Id, eventCreateRequest));
        
        Assert.Equal("Address must contain street and postal code separated by a comma", exception.Message);
    }

    [Fact]
    public async Task ReactToEventAsync_ShouldAddUser_WhenNotAlreadyInterested()
    {
        // Arrange
        const int eventId = 1;
        const string auth0Id = "auth0|123";
        var eventEntity = new Event
        {
            Id = eventId,
            Title = "Test Event",
            Body = "Test Body",
            City = "Test City",
            Address = "123 Test St",
            Latitude = 0,
            Longitude = 0,
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow.AddHours(2),
            EventCategory = EventCategory.Other,
            InterestedUsers = new List<User>()
        };
        var user = new User { Auth0Id = auth0Id, FullName = "Test User" };

        _eventRepositoryMock.Setup(er => er.GetAsync(
                It.IsAny<Expression<Func<Event, bool>>>(), 
                It.IsAny<bool>(), 
                It.IsAny<Func<IQueryable<Event>, IQueryable<Event>>>()))
            .ReturnsAsync(eventEntity);
        _userServiceMock.Setup(us => us.GetUserByAuth0IdAsync(auth0Id, true)).ReturnsAsync(user);

        // Act
        await _eventService.ReactToEventAsync(eventId, auth0Id);

        // Assert
        Assert.Contains(user, eventEntity.InterestedUsers);
    }

    [Fact]
    public async Task ReactToEventAsync_ShouldRemoveUser_WhenAlreadyInterested()
    {
        // Arrange
        const int eventId = 1;
        const string auth0Id = "auth0|123";
        var user = new User { Auth0Id = auth0Id, FullName = "Test User" };

        var eventEntity = new Event
        {
            Id = eventId,
            Title = "Test Event",
            Body = "Test Body",
            City = "Test City",
            Address = "123 Test St",
            Latitude = 0,
            Longitude = 0,
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow.AddHours(2),
            EventCategory = EventCategory.Other,
            InterestedUsers = new List<User> { user }
        };

        _eventRepositoryMock.Setup(er => er.GetAsync(
                It.IsAny<Expression<Func<Event, bool>>>(), 
                It.IsAny<bool>(), 
                It.IsAny<Func<IQueryable<Event>, IQueryable<Event>>>()))
            .ReturnsAsync(eventEntity);

        // Act
        await _eventService.ReactToEventAsync(eventId, auth0Id);

        // Assert
        Assert.DoesNotContain(user, eventEntity.InterestedUsers);
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldDeleteEvent_WhenAuthorized()
    {
        // Arrange
        const int eventId = 1;
        const string auth0Id = "auth0|123";
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
        var eventEntity = new Event
        {
            Id = eventId,
            Establishment = establishment,
            Title = "Test Event",
            Body = "Test Body",
            City = "Test City",
            Address = "123 Test St",
            Latitude = 0,
            Longitude = 0,
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow.AddHours(2),
            EventCategory = EventCategory.Other,
            InterestedUsers = new List<User>()
        };

        _eventRepositoryMock.Setup(er => er.GetAsync(
                It.IsAny<Expression<Func<Event, bool>>>(), 
                It.IsAny<bool>(), 
                It.IsAny<Func<IQueryable<Event>, IQueryable<Event>>>()))
            .ReturnsAsync(eventEntity);
        _eventRepositoryMock.Setup(er => er.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);
        
        // Act
        await _eventService.DeleteEventAsync(auth0Id, eventId);

        // Assert
        _eventRepositoryMock.Verify(er => er.DeleteAsync(eventId), Times.Once);
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldThrowUnauthorizedAccessException_WhenNotAuthorized()
    {
        // Arrange
        const int eventId = 1;
        const string auth0Id = "auth0|123";
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
        var eventEntity = new Event
        {
            Id = eventId,
            EstablishmentId = 2,
            Title = "Test Event",
            Body = "Test Body",
            City = "Test City",
            Address = "123 Test St",
            Latitude = 0,
            Longitude = 0,
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow.AddHours(2),
            EventCategory = EventCategory.Other,
            InterestedUsers = new List<User>()
        };

        _eventRepositoryMock.Setup(er => er.GetAsync(
                It.IsAny<Expression<Func<Event, bool>>>(), 
                It.IsAny<bool>(), 
                It.IsAny<Func<IQueryable<Event>, IQueryable<Event>>>()))
            .ReturnsAsync(eventEntity);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _eventService.DeleteEventAsync(auth0Id, eventId));

        Assert.Equal("Establishment does not own this event", exception.Message);
    }
}
