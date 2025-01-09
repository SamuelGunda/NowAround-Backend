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
        var storageAccount = configuration.GetConnectionString("StorageAccount") ?? throw new ArgumentNullException(configuration.GetConnectionString("StorageAccount"));
        
        var credential = new StorageSharedKeyCredential(storageAccount, storageKey);
        _blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccount}.blob.core.windows.net"), credential);
    }
    
    public async Task<string> UploadPictureAsync(IFormFile file, string role, string auth0Id, string imageContext, int? contextId)
    {
        var sanitizedAuth0Id = auth0Id.Replace("|", "-").ToLower();
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(role.ToLower());
        
        await containerClient.CreateIfNotExistsAsync();

        string blobPath;
        
        var contentType = file.ContentType;
        var fileFormat = contentType.Split("/")[1];
        
        if (imageContext is "profile-picture" or "background-picture")
        {
            blobPath = $"{sanitizedAuth0Id}/{imageContext}";
        }
        else
        {
            ArgumentNullException.ThrowIfNull(contextId);
            blobPath = $"{sanitizedAuth0Id}/{imageContext}/{contextId}";
        }
        
        blobPath += "." + fileFormat;
        
        var blobClient = containerClient.GetBlobClient(blobPath);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
        
        return $"https://nowaroundimagestorage.blob.core.windows.net/{role.ToLower()}/{blobPath}";
    }

    public async Task DeleteAsync(string role, string auth0Id, string imageContext)
    {
        var sanitizedAuth0Id = auth0Id.Replace("|", "-").ToLower();

        var containerClient = _blobServiceClient.GetBlobContainerClient(role.ToLower());

        var blobPath =  $"{sanitizedAuth0Id}/{imageContext}";
        var blobClient = containerClient.GetBlobsAsync(prefix: blobPath);

        await foreach (var blobItem in blobClient)
        {
            await containerClient.GetBlobClient(blobItem.Name).DeleteAsync();
        }
    }

    public void CheckPictureType(string contentType)
    {
        var permittedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        
        if (!permittedImageTypes.Contains(contentType))
        {
            throw new ArgumentException("Invalid image type");
        }
    }
}