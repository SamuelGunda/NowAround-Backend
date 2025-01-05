using Azure.Storage;
using Azure.Storage.Blobs;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Services;

public class StorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    /*private readonly IEstablishmentService _establishmentService;
    private readonly IUserService _userService;*/
    
     public StorageService(IConfiguration configuration/*, IEstablishmentService establishmentService, IUserService userService*/)
    {
        /*_establishmentService = establishmentService;
        _userService = userService;*/
        
        var storageKey = configuration.GetConnectionString("StorageKey") ?? throw new ArgumentNullException(configuration.GetConnectionString("StorageKey"));
        var storageAccount = configuration.GetConnectionString("StorageAccount") ?? throw new ArgumentNullException(configuration.GetConnectionString("StorageAccount"));
        
        var credential = new StorageSharedKeyCredential(storageAccount, storageKey);
        _blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccount}.blob.core.windows.net"), credential);
    }
    
    public async Task<string> UploadPictureAsync(IFormFile file, string role, string auth0Id, string imageContext, int? contextId)
    {
        var sanitizedAuth0Id = auth0Id.Replace("|", "-").ToLower();
        
        /*
        var accountExists = await _userService.CheckIfUserExistsAsync(auth0Id) || await _establishmentService.CheckIfEstablishmentExistsAsync(auth0Id);
        
        if (!accountExists)
        {
            throw new EntityNotFoundException("Account", "Auth0 ID", auth0Id);
        }
        */

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
        
        /*
        var imageUrl = blobClient.Uri.ToString();
        await AssignImageUrlToEntity(role, auth0Id, imageContext, contextId, imageUrl);
        */
        
        return "https://nowaroundimagestorage.blob.core.windows.net/" + blobPath;
    }
    
    /*private async Task AssignImageUrlToEntity(string role, string auth0Id, string imageContext, int? contextId, string imageUrl)
    {
        switch (role)
        {
            case "User":
                await _userService.UpdateUserPictureAsync(auth0Id, imageUrl);
                break;
            case "Establishment":
                await _establishmentService.UpdateEstablishmentPictureAsync(auth0Id, imageUrl);
                break;
            default:
                throw new ArgumentException("Invalid role");
        }
    }*/
    
    public void CheckPictureType(string contentType)
    {
        var permittedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        
        if (!permittedImageTypes.Contains(contentType))
        {
            throw new ArgumentException("Invalid image type");
        }
    }
}