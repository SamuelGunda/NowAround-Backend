using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NowAround.Api.Apis.Auth0.Exceptions;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Apis.Mapbox.Interfaces;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Models.Responses;

namespace NowAround.Api.IntegrationTests.Controllers;

public class EstablishmentControllerTests
{
    private readonly Mock<IAuth0Service> _auth0ServiceMock;
    private readonly Mock<IMapboxService> _mapboxServiceMock;

    public EstablishmentControllerTests()
    {
        _auth0ServiceMock = new Mock<IAuth0Service>();
        _mapboxServiceMock = new Mock<IMapboxService>();
    }
    
    // RegisterEstablishmentAsync Tests

    [Fact]
    public async Task RegisterEstablishmentAsync_WithValidRequest_ShouldReturnCreated()
    {
        _auth0ServiceMock
            .Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<PersonalInfo>()))
            .ReturnsAsync("auth0|valid_register");

        _mapboxServiceMock
            .Setup(s => s.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((0.0, 0.0));
                
        // Arrange
        var factory = new NowAroundWebApplicationFactory(_auth0ServiceMock, _mapboxServiceMock);

        var client = factory.CreateClient();

        var establishmentRegisterRequest = new EstablishmentRegisterRequest
        {
            EstablishmentInfo = new EstablishmentInfo
            {
                Name = "Test Bar",
                Address = "Jilemnickeho 9",
                PostalCode = "965 01",
                City = "Žiar nad Hronom",
                PriceCategory = 1,
                Category = new List<string> { "BAR" },
                Tags = new List<string> { "PET_FRIENDLY" }
            },
            PersonalInfo = new PersonalInfo
            {
                FirstName = "Test",
                LastName = "Bar",
                Email = "EstablishmentTest@Test.com"
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(establishmentRegisterRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/establishment", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task RegisterEstablishmentAsync_WithInvalidRequest_ShouldReturnBadRequest()
    {
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();

        var establishmentRegisterRequest = new EstablishmentRegisterRequest
        {
            EstablishmentInfo = new EstablishmentInfo
            {
                Name = "Test Restaurant",
                Address = "Jilemnickeho 9",
                PostalCode = "965 01",
                City = "Žiar nad Hronom",
                PriceCategory = 1,
                Category = new List<string> { "BAR" },
                Tags = new List<string> { "PET_FRIENDLY" }
            },
            PersonalInfo = new PersonalInfo
            {
                FirstName = "Test",
                LastName = "Bar"
            }
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(establishmentRegisterRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/establishment", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task RegisterEstablishmentAsync_WithDuplicateName_ShouldReturnConflict()
    {
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();

        var establishmentRegisterRequest = new EstablishmentRegisterRequest
        {
            EstablishmentInfo = new EstablishmentInfo
            {
                Name = "Test Restaurant",
                Address = "Jilemnickeho 9",
                PostalCode = "965 01",
                City = "Žiar nad Hronom",
                PriceCategory = 1,
                Category = new List<string> { "BAR" },
                Tags = new List<string> { "PET_FRIENDLY" }
            },
            PersonalInfo = new PersonalInfo
            {
                FirstName = "Test",
                LastName = "Bar",
                Email = "EstablishmentTest@Test.com"
            }
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(establishmentRegisterRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/establishment", content);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task RegisterEstablishmentAsync_WithEmailTaken_ShouldReturnConflict()
    {
        _auth0ServiceMock
            .Setup(s => s.RegisterEstablishmentAccountAsync(It.IsAny<PersonalInfo>()))
            .ThrowsAsync(new EmailAlreadyInUseException("Email already in use"));

        var factory = new NowAroundWebApplicationFactory(_auth0ServiceMock);

        var client = factory.CreateClient();

        var establishmentRegisterRequest = new EstablishmentRegisterRequest
        {
            EstablishmentInfo = new EstablishmentInfo
            {
                Name = "Test Restaurant",
                Address = "Jilemnickeho 9",
                PostalCode = "965 01",
                City = "Žiar nad Hronom",
                PriceCategory = 1,
                Category = new List<string> { "BAR" },
                Tags = new List<string> { "PET_FRIENDLY" }
            },
            PersonalInfo = new PersonalInfo
            {
                FirstName = "Test",
                LastName = "Bar",
                Email = "john.doe@example.com"
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(establishmentRegisterRequest), Encoding.UTF8, "application/json");
        
        // Act
        var response = await client.PostAsync("/api/establishment", content);
        
        // Assert
        
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    // GetEstablishmentByAuth0IdAsync Tests
    
    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_WithValidAuth0Id_ShouldReturnEstablishment()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        const string auth0Id = "auth0|valid";
        var establishment = new EstablishmentResponse
        {
            Auth0Id = auth0Id,
            Name = "Test Restaurant",
            Description = "Test Description",
            Address = "123 Test St",
            City = "Test City",
            Latitude = 0,
            Longitude = 0,
            PriceCategory = PriceCategory.Affordable,
            RequestStatus = RequestStatus.Accepted,
            CategoryNames = new List<string> { "RESTAURANT" },
            TagNames = new List<string> { "PET_FRIENDLY" }
        };

        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/establishment/{auth0Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var establishmentResponse = JsonConvert.DeserializeObject<EstablishmentResponse>(responseContent);

        Assert.Equal(establishment.Auth0Id, establishmentResponse.Auth0Id);
        Assert.Equal(establishment.Name, establishmentResponse.Name);
        Assert.Equal(establishment.Description, establishmentResponse.Description);
        Assert.Equal(establishment.Address, establishmentResponse.Address);
        Assert.Equal(establishment.City, establishmentResponse.City);
        Assert.Equal(establishment.Latitude, establishmentResponse.Latitude);
        Assert.Equal(establishment.Longitude, establishmentResponse.Longitude);
    }
    
    [Fact]
    public async Task GetEstablishmentByAuth0IdAsync_WithInvalidAuth0Id_ShouldReturnNotFound()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        const string auth0Id = "auth0|invalid";
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/establishment/{auth0Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    // GetPendingEstablishmentsAsync Tests
    
    [Fact]
    public async Task GetPendingEstablishmentsAsync_WithPendingEstablishments_ShouldReturnEstablishments()
    {
        // Arrange
        _auth0ServiceMock
            .Setup(s => s.GetEstablishmentOwnerFullNameAsync(It.IsAny<string>()))
            .ReturnsAsync("Test Owner");
        
        var factory = new NowAroundWebApplicationFactory(_auth0ServiceMock);

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var pendingEstablishment = new Establishment
            {
                Auth0Id = "auth0|pending",
                Name = "Pending Restaurant",
                Description = "Pending Description",
                Address = "456 Test St",
                City = "Test City",
                Latitude = 10,
                Longitude = 20,
                PriceCategory = PriceCategory.Expensive,
                RequestStatus = RequestStatus.Pending,
            };
            dbContext.Establishments.Add(pendingEstablishment);
            await dbContext.SaveChangesAsync();
        }
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");
        
        // Act
        var response = await client.GetAsync("/api/establishment/pending");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var establishments = JsonConvert.DeserializeObject<List<EstablishmentResponse>>(responseContent);

        Assert.NotEmpty(establishments);
    }
    
    [Fact]
    public async Task GetPendingEstablishmentsAsync_WithNoPendingEstablishments_ShouldReturnNoContent()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");

        // Act
        var response = await client.GetAsync("/api/establishment/pending");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task GetPendingEstablishmentsAsync_IncorrectRole_ShouldReturnForbidden()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "User");

        // Act
        var response = await client.GetAsync("/api/establishment/pending");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact]
    public async Task GetPendingEstablishmentsAsync_WithUnauthorizedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/establishment/pending");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    // GetEstablishmentMarkersWithFilterAsync Tests
    
    [Fact]
    public async Task GetEstablishmentMarkersWithFilterAsync_WithValidCategory_ShouldReturnEstablishment()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/establishment/search?categoryName=RESTAURANT");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var establishments = JsonConvert.DeserializeObject<List<EstablishmentResponse>>(responseContent);

        Assert.NotEmpty(establishments);
        Assert.Single(establishments);
    }
    
    [Fact]
    public async Task GetEstablishmentMarkersWithFilterAsync_WithValidTags_ShouldReturnEstablishment()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/establishment/search?tagNames=PET_FRIENDLY,FAMILY_FRIENDLY");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var establishments = JsonConvert.DeserializeObject<List<EstablishmentResponse>>(responseContent);

        Assert.NotEmpty(establishments);
        Assert.Single(establishments);
    }
    
    [Fact]
    public async Task GetEstablishmentMarkersWithFilterAsync_WithValidName_ShouldReturnEstablishments()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/establishment/search?name=Test");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var establishments = JsonConvert.DeserializeObject<List<EstablishmentResponse>>(responseContent);

        Assert.NotEmpty(establishments);
        Assert.Equal(2, establishments.Count);
    }
    
    [Fact]
    public async Task GetEstablishmentMarkersWithFilterAsync_WithNoMatchingName_ShouldReturnNoContent()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/establishment/search?name=Unknown");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task GetEstablishmentMarkersWithFilterAsync_WithInvalidPriceCategory_ShouldReturnInternalServerError()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/establishment/search?priceCategory=5");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task GetEstablishmentMarkersWithFilterAsync_WithNoFilters_ShouldReturnInternalServerError()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/establishment/search");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    // GetEstablishmentMarkersWithFilterInAreaAsync Tests
    
    [Fact]
    public async Task GetEstablishmentMarkersWithFilterInAreaAsync_WithValidBounds_ShouldReturnEstablishments()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/establishment/search-area?northWestLat=3&northWestLong=0&southEastLat=0&southEastLong=3");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var establishments = JsonConvert.DeserializeObject<List<EstablishmentResponse>>(responseContent);

        Assert.NotEmpty(establishments);
        Assert.Equal(2, establishments.Count);
    }
    
    [Fact]
    public async Task GetEstablishmentMarkersWithFilterInAreaAsync_WhenNoEstablishmentsInArea_ShouldReturnNoContent()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/establishment/search-area?northWestLat=50&northWestLong=49&southEastLat=49&southEastLong=50");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task GetEstablishmentMarkersWithFilterInAreaAsync_WithInvalidBounds_ShouldReturnInternalServerError()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/establishment/search-area?northWestLat=0&northWestLong=0&southEastLat=0&southEastLong=0");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task GetEstablishmentMarkersWithFilterInAreaAsync_WithValidBoundsAndFilters_ShouldReturnEstablishments()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/establishment/search-area?northWestLat=3&northWestLong=0&southEastLat=0&southEastLong=3&name=Test&priceCategory=0&categoryName=RESTAURANT&tagNames=PET_FRIENDLY");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var establishments = JsonConvert.DeserializeObject<List<EstablishmentResponse>>(responseContent);

        Assert.NotEmpty(establishments);
        Assert.Single(establishments);
    }
    
    // UpdateEstablishmentAsync Tests
    
    [Fact]
    public async Task UpdateEstablishmentAsync_WithValidRequest_ShouldReturnNoContent()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");

        var establishmentUpdateRequest = new EstablishmentUpdateRequest
        {
            Auth0Id = "auth0|valid",
            Name = "Updated Restaurant",
            Description = "Updated Description",
            PriceCategory = 2,
            Category = new List<string> { "RESTAURANT" },
            Tags = new List<string> { "PET_FRIENDLY" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(establishmentUpdateRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/establishment", content);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateEstablishmentAsync_WithInvalidRequest_ShouldReturnNoContent()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|valid");

        var establishmentUpdateRequest = new EstablishmentUpdateRequest
        {
            Auth0Id = "auth0|valid",
            Name = "Updated Restaurant",
            Description = "Updated Description",
            PriceCategory = 2,
            Category = new List<string> { "RESTAURANT" },
            Tags = new List<string> { "PET_FRIENDLY" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(establishmentUpdateRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/establishment", content);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateEstablishmentAsync_WithUnauthorizedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();

        var establishmentUpdateRequest = new EstablishmentUpdateRequest
        {
            Auth0Id = "auth0|valid",
            Name = "Updated Restaurant",
            Description = "Updated Description",
            PriceCategory = 2,
            Category = new List<string> { "RESTAURANT" },
            Tags = new List<string> { "PET_FRIENDLY" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(establishmentUpdateRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/establishment", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateEstablishmentAsync_WithInvalidSub_ShouldReturnForbidden()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|invalid");

        var establishmentUpdateRequest = new EstablishmentUpdateRequest
        {
            Auth0Id = "auth0|valid",
            Name = "Updated Restaurant",
            Description = "Updated Description",
            PriceCategory = 2,
            Category = new List<string> { "RESTAURANT" },
            Tags = new List<string> { "PET_FRIENDLY" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(establishmentUpdateRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/establishment", content);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateEstablishmentAsync_WithInvalidAuth0Id_ShouldReturnNotFound()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");

        var establishmentUpdateRequest = new EstablishmentUpdateRequest
        {
            Auth0Id = "auth0|invalid",
            Name = "Updated Restaurant",
            Description = "Updated Description",
            PriceCategory = 2,
            Category = new List<string> { "RESTAURANT" },
            Tags = new List<string> { "PET_FRIENDLY" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(establishmentUpdateRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/establishment", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    // UpdateEstablishmentRegisterRequestAsync Tests
    
    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_WithValidRequest_ShouldReturnNoContent()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var pendingEstablishment = new Establishment
            {
                Auth0Id = "auth0|pending",
                Name = "Pending Restaurant",
                Description = "Pending Description",
                Address = "456 Test St",
                City = "Test City",
                Latitude = 10,
                Longitude = 20,
                PriceCategory = PriceCategory.Expensive,
                RequestStatus = RequestStatus.Pending,
            };
            dbContext.Establishments.Add(pendingEstablishment);
            await dbContext.SaveChangesAsync();
        }
        
        var client = factory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");

        // Act
        var response = await client.PutAsync("/api/establishment/register-status?auth0Id=auth0|pending&action=accept", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_WithInvalidAuth0Id_ShouldReturnNotFound()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");

        // Act
        var response = await client.PutAsync("/api/establishment/register-status?auth0Id=auth0|invalid&action=accept", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_WithInvalidAction_ShouldReturnBadRequest()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");

        // Act
        var response = await client.PutAsync("/api/establishment/register-status?auth0Id=auth0|pending&action=invalid", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_WithUnauthorizedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();

        // Act
        var response = await client.PutAsync("/api/establishment/register-status?auth0Id=auth0|pending&action=accept", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateEstablishmentRegisterRequestAsync_WithInvalidRole_ShouldReturnForbidden()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "User");

        // Act
        var response = await client.PutAsync("/api/establishment/register-status?auth0Id=auth0|pending&action=accept", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    // DeleteEstablishmentAsync Tests
    
    [Fact]
    public async Task DeleteEstablishmentAsync_WithValidAuth0Id_ShouldReturnNoContent()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");

        // Act
        var response = await client.DeleteAsync("/api/establishment?auth0Id=auth0|valid");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteEstablishmentAsync_WithValidSub_ShouldReturnNoContent()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|valid");

        // Act
        var response = await client.DeleteAsync("/api/establishment?auth0Id=auth0|valid");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteEstablishmentAsync_WithInvalidAuth0Id_ShouldReturnNotFound()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Admin");

        // Act
        var response = await client.DeleteAsync("/api/establishment?auth0Id=auth0|invalid");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteEstablishmentAsync_WithUnauthorizedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/establishment?auth0Id=auth0|valid");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteEstablishmentAsync_WithInvalidSub_ShouldReturnForbidden()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|invalid");

        // Act
        var response = await client.DeleteAsync("/api/establishment?auth0Id=auth0|valid");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}