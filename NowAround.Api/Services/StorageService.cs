using Azure.Storage;
using Azure.Storage.Blobs;
using NowAround.Api.Exceptions;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Services;

public class StorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IEstablishmentService _establishmentService;
    private readonly IUserService _userService;
    
     public StorageService(IConfiguration configuration, IEstablishmentService establishmentService, IUserService userService)
    {
        
        _establishmentService = establishmentService;
        _userService = userService;
        
        var storageKey = configuration.GetConnectionString("StorageKey") ?? throw new ArgumentNullException(configuration.GetConnectionString("StorageKey"));
        var storageAccount = configuration.GetConnectionString("StorageAccount") ?? throw new ArgumentNullException(configuration.GetConnectionString("StorageAccount"));
        
        var credential = new StorageSharedKeyCredential(storageAccount, storageKey);
        _blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccount}.blob.core.windows.net"), credential);
        
    }
    
    public async Task<string> UploadImageAsync(IFormFile file, string role, string auth0Id, string imageContext, string contextId)
    {
        var permittedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        var sanitizedAuth0Id = auth0Id.Replace("|", "-").ToLower();

        if (!permittedImageTypes.Contains(file.ContentType))
        {
            throw new ArgumentException("Invalid image type");
        }
        
        var accountExists = await _userService.CheckIfUserExistsAsync(auth0Id) || await _establishmentService.CheckIfEstablishmentExistsAsync(auth0Id);
        
        if (!accountExists)
        {
            throw new EntityNotFoundException("Account", "Auth0 ID", auth0Id);
        }

        var containerClient = _blobServiceClient.GetBlobContainerClient(role.ToLower());
        await containerClient.CreateIfNotExistsAsync();

        string blobPath;
        
        if (imageContext is "profile-picture" or "cover-picture")
        {
            blobPath = $"{sanitizedAuth0Id}/{imageContext}/{file.FileName}";
        }
        else
        {
            ArgumentNullException.ThrowIfNull(contextId);
            blobPath = $"{sanitizedAuth0Id}/{imageContext}/{contextId}/{file.FileName}";
        }
        
        var blobClient = containerClient.GetBlobClient(blobPath);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
        
        var imageUrl = blobClient.Uri.ToString();
        await AssignImageUrlToEntity(role, auth0Id, imageContext, contextId, imageUrl);
        
        return "https://nowaroundimagestorage.blob.core.windows.net/" + blobPath;
    }
    
    private async Task AssignImageUrlToEntity(string role, string auth0Id, string imageContext, string contextId, string imageUrl)
    {
        switch (imageContext)
        {
            case "profile-picture":
            case "cover-picture":
                if (role == "User")
                {
                    await _userService.UpdateUserPictureAsync(auth0Id, imageUrl);
                }
                else
                {
                    await _establishmentService.UpdateEstablishmentPictureAsync(auth0Id, imageUrl);
                }
                break;
            case "post":
                Console.WriteLine($"Post image uploaded: {imageUrl}");
                break;
            case "event":
                break;
            case "menu-item":
                break;
        }
    }
}