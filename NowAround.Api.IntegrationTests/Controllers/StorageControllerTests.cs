using System.Net;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace NowAround.Api.IntegrationTests.Controllers;

public class StorageControllerTests : IClassFixture<StorageContextFixture>
{
    private readonly BlobServiceClient _blobServiceClient;
    private string? _containerName;
    private string? _blobPath;

    public StorageControllerTests(StorageContextFixture fixture)
    {
        _blobServiceClient = fixture.StorageContext.BlobServiceClient;
    }

    private async Task CleanStorage()
    {
        if (!string.IsNullOrEmpty(_containerName) && !string.IsNullOrEmpty(_blobPath))
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(_blobPath);
            await blobClient.DeleteIfExistsAsync();
        }
    }
    
    [Fact]
    public async Task UploadImageAsync_WithValidUserProfilePictureRequest_ShouldReturnCreated()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "User auth0|valid");

        var fileBytes = "Test image"u8.ToArray();
        var imageContent = new ByteArrayContent(fileBytes);
        
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        imageContent.Headers.ContentLength = fileBytes.Length;
    
        var testFile = new MultipartFormDataContent();
        testFile.Add(imageContent, "image", "test.jpg");

        const string requestUrl = "/api/Storage/upload/profile-picture";

        // Act
        var response = await client.PostAsync(requestUrl, testFile);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Image successfully uploaded", await response.Content.ReadAsStringAsync());
        Assert.Equal("https://nowaroundimagestorage.blob.core.windows.net/auth0-valid/profile-picture", response.Headers.Location?.ToString());
        
        _containerName = "user";
        _blobPath = "auth0-valid/profile-picture";
        await CleanStorage();
    }


    [Fact]
    public async Task UploadImageAsync_WithValidEstablishmentProfilePictureRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|valid");

        var fileBytes = "Test image"u8.ToArray();
        var imageContent = new ByteArrayContent(fileBytes);
        
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        imageContent.Headers.ContentLength = fileBytes.Length;
    
        var testFile = new MultipartFormDataContent();
        testFile.Add(imageContent, "image", "test.jpg");

        const string requestUrl = "/api/Storage/upload/profile-picture";

        // Act
        var response = await client.PostAsync(requestUrl, testFile);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Image successfully uploaded", await response.Content.ReadAsStringAsync());
        Assert.Equal("https://nowaroundimagestorage.blob.core.windows.net/auth0-valid/profile-picture", response.Headers.Location?.ToString());
        
        _containerName = "establishment";
        _blobPath = "auth0-valid/profile-picture";
        await CleanStorage();
    }
    
    [Fact]
    public async Task UploadImageAsync_WithValidPostRequest_ShouldReturnCreated()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "User auth0|valid");

        var fileBytes = "Test image"u8.ToArray();
        var imageContent = new ByteArrayContent(fileBytes);
        
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        imageContent.Headers.ContentLength = fileBytes.Length;
    
        var testFile = new MultipartFormDataContent();
        testFile.Add(imageContent, "image", "test.jpg");

        const string requestUrl = "/api/Storage/upload/post?id=1";

        // Act
        var response = await client.PostAsync(requestUrl, testFile);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Image successfully uploaded", await response.Content.ReadAsStringAsync());
        Assert.Equal("https://nowaroundimagestorage.blob.core.windows.net/auth0-valid/post/1", response.Headers.Location?.ToString());
        
        _containerName = "user";
        _blobPath = "auth0-valid/post/1";
        await CleanStorage();
    }
    
    [Fact]
    public async Task UploadImageAsync_WithInvalidProfilePictureRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "User auth0|invalid");

        var fileBytes = "Test image"u8.ToArray();
        var imageContent = new ByteArrayContent(fileBytes);
        
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        imageContent.Headers.ContentLength = fileBytes.Length;
    
        var testFile = new MultipartFormDataContent();
        testFile.Add(imageContent, "image", "test.jpg");

        const string requestUrl = "/api/Storage/upload/profile-picture";

        // Act
        var response = await client.PostAsync(requestUrl, testFile);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task UploadImageAsync_WithInvalidImageType_ShouldReturnBadRequest()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "User auth0|valid");

        var fileBytes = "Test image"u8.ToArray();
        var imageContent = new ByteArrayContent(fileBytes);
        
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        imageContent.Headers.ContentLength = fileBytes.Length;
    
        var testFile = new MultipartFormDataContent();
        testFile.Add(imageContent, "image", "test.jpg");

        const string requestUrl = "/api/Storage/upload/profile-picture";

        // Act
        var response = await client.PostAsync(requestUrl, testFile);
        
        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task UploadImageAsync_WithUnauthorizedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();
        
        var fileBytes = "Test image"u8.ToArray();
        var imageContent = new ByteArrayContent(fileBytes);
        
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        imageContent.Headers.ContentLength = fileBytes.Length;
    
        var testFile = new MultipartFormDataContent();
        testFile.Add(imageContent, "image", "test.jpg");

        const string requestUrl = "/api/Storage/upload/profile-picture";

        // Act
        var response = await client.PostAsync(requestUrl, testFile);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task UploadImageAsync_NotOwnedPost_ShouldReturnUnauthorized()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "User auth0|valid2");

        var fileBytes = "Test image"u8.ToArray();
        var imageContent = new ByteArrayContent(fileBytes);
        
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        imageContent.Headers.ContentLength = fileBytes.Length;
    
        var testFile = new MultipartFormDataContent();
        testFile.Add(imageContent, "image", "test.jpg");

        const string requestUrl = "/api/Storage/upload/post?id=1";

        // Act
        var response = await client.PostAsync(requestUrl, testFile);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
}