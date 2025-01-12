using System.Net;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using NowAround.Api.Models.Requests;

namespace NowAround.Api.IntegrationTests.Controllers;

public class PostControllerTests  : IClassFixture<StorageContextFixture>
{
    private readonly BlobServiceClient _blobServiceClient;
    private string? _containerName;
    private string? _blobPath;

    public PostControllerTests(StorageContextFixture fixture)
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
    
    // CreatePostAsync tests
    
    [Fact]
    public async Task CreatePostAsync_ForValidRequestWithoutPicture_ShouldReturnCreated()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|valid");

        var postCreateRequest = new PostCreateUpdateRequest
        {
            Headline = "Test headline",
            Body = "Test body"
        };

        var content = new MultipartFormDataContent
        {
            { new StringContent(postCreateRequest.Headline), "Headline" },
            { new StringContent(postCreateRequest.Body), "Body" }
        };

        // Act
        var response = await client.PostAsync("/api/post", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task CreatePostAsync_ForValidRequestWithPicture_ShouldReturnCreated()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|valid");

        var postCreateRequest = new PostCreateUpdateRequest
        {
            Headline = "Test headline",
            Body = "Test body",
            Picture = new FormFile(new MemoryStream("Test picture"u8.ToArray()), 0, "Test picture".Length, "Picture", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            }
        };

        var content = new MultipartFormDataContent
        {
            { new StringContent(postCreateRequest.Headline), "Headline" },
            { new StringContent(postCreateRequest.Body), "Body" }
        };

        var pictureContent = new StreamContent(postCreateRequest.Picture.OpenReadStream());
        pictureContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(postCreateRequest.Picture.ContentType);
        content.Add(pictureContent, "Picture", postCreateRequest.Picture.FileName);

        // Act
        var response = await client.PostAsync("/api/post", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        if (response.StatusCode == HttpStatusCode.Created)
        {
            _containerName = "establishment";
            _blobPath = "auth0-valid/post/2";
            await CleanStorage();
        }
    }
    
    [Fact]
    public async Task CreatePostAsync_ForInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|valid");

        var postCreateRequest = new PostCreateUpdateRequest
        {
            Headline = "Test headline",
            Body = "Test body"
        };

        var content = new MultipartFormDataContent
        {
            { new StringContent(postCreateRequest.Headline), "Headline" }
        };

        // Act
        var response = await client.PostAsync("/api/post", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    // GetPostAsync tests
    
    [Fact]
    public async Task GetPostAsync_ForValidPostId_ShouldReturnOk()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();

        const int postId = 1;

        // Act
        var response = await client.GetAsync($"/api/post/{postId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetPostAsync_ForInvalidPostId_ShouldReturnNotFound()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();

        const int postId = 999;

        // Act
        var response = await client.GetAsync($"/api/post/{postId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    // DeletePostAsync tests
    
    [Fact]
    public async Task DeletePostAsync_ForValidPostId_ShouldReturnOk()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|valid");

        const int postId = 1;

        // Act
        var response = await client.DeleteAsync($"/api/post/{postId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task DeletePostAsync_ForInvalidPostId_ShouldReturnNotFound()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|valid");

        const int postId = 999;

        // Act
        var response = await client.DeleteAsync($"/api/post/{postId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task DeletePostAsync_ForUnauthorizedUser_ShouldReturnForbidden()
    {
        // Arrange
        var factory = new NowAroundWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Establishment auth0|invalid");

        const int postId = 1;

        // Act
        var response = await client.DeleteAsync($"/api/post/{postId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}