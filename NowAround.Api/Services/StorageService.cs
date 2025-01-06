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
        
        if (imageContext is "profile-picture" or "background-picture")
        {
            blobPath = $"{sanitizedAuth0Id}/{imageContext}";
        }
        else
        {
            ArgumentNullException.ThrowIfNull(contextId);
            blobPath = $"{sanitizedAuth0Id}/{imageContext}/{contextId}";
        }
        
        var blobClient = containerClient.GetBlobClient(blobPath);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
        
        return "https://nowaroundimagestorage.blob.core.windows.net/" + blobPath;
    }
    
    public async Task DeleteAccountFolderAsync(string role, string auth0Id)
    {
        var sanitizedAuth0Id = auth0Id.Replace("|", "-").ToLower();
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(role.ToLower());
        
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: sanitizedAuth0Id + "/"))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);

            await blobClient.DeleteAsync();
        }
    }

    public async Task DeletePictureAsync(string role, string auth0Id, string imageContext, int? contextId)
    {
        var sanitizedAuth0Id = auth0Id.Replace("|", "-").ToLower();
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(role.ToLower());
        var blobPath = contextId == null ? $"{sanitizedAuth0Id}/{imageContext}" : $"{sanitizedAuth0Id}/{imageContext}/{contextId}";
        var blobClient = containerClient.GetBlobClient(blobPath);
        
        await blobClient.DeleteIfExistsAsync();
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