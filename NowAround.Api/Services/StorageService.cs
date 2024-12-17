using System.Reflection.Metadata;
using Azure.Storage;
using Azure.Storage.Blobs;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Services;

public class StorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    
     public StorageService(IConfiguration configuration)
    {
        var storageKey = configuration.GetConnectionString("StorageKey") ?? throw new ArgumentNullException(configuration.GetConnectionString("StorageKey"));
        var credential = new StorageSharedKeyCredential("nowaroundimagestorage", storageKey);
        _blobServiceClient = new BlobServiceClient(new Uri($"https://nowaroundimagestorage.blob.core.windows.net"), credential);
    }
    
    public async Task UploadImageAsync(IFormFile file, string role, string auth0Id, string imageContext, string contextId)
    {
        var permittedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        auth0Id = auth0Id.Replace("|", "-").ToLower();

        if (!permittedImageTypes.Contains(file.ContentType))
        {
            throw new ArgumentException("Invalid image type");
        }

        var containerClient = _blobServiceClient.GetBlobContainerClient(role.ToLower());
        await containerClient.CreateIfNotExistsAsync();

        var blobPath = $"{auth0Id}/{imageContext}/{contextId}/{file.FileName}";
        var blobClient = containerClient.GetBlobClient(blobPath);

        await using var stream = file.OpenReadStream();
        
        await blobClient.UploadAsync(stream, overwrite: true);
    }
}