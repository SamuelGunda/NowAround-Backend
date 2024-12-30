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
    private readonly IPostService _postService;
    
     public StorageService(IConfiguration configuration, IEstablishmentService establishmentService, IUserService userService, IPostService postService)
    {
        _establishmentService = establishmentService;
        _userService = userService;
        _postService = postService;
        
        var storageKey = configuration.GetConnectionString("StorageKey") ?? throw new ArgumentNullException(configuration.GetConnectionString("StorageKey"));
        var storageAccount = configuration.GetConnectionString("StorageAccount") ?? throw new ArgumentNullException(configuration.GetConnectionString("StorageAccount"));
        
        var credential = new StorageSharedKeyCredential(storageAccount, storageKey);
        _blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccount}.blob.core.windows.net"), credential);
    }
    
    public async Task<string> UploadImageAsync(IFormFile file, string role, string auth0Id, string imageContext, int? contextId)
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
        
        var imageUrl = blobClient.Uri.ToString();
        await AssignImageUrlToEntity(role, auth0Id, imageContext, contextId, imageUrl);
        
        return "https://nowaroundimagestorage.blob.core.windows.net/" + blobPath;
    }
    
    private async Task AssignImageUrlToEntity(string role, string auth0Id, string imageContext, int? contextId, string imageUrl)
    {
        if (contextId == null)
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
        }
        else
        {
            switch (imageContext)
            {
                case "post":
                    if (!await _postService.CheckPostOwnershipByAuth0IdAsync(auth0Id, contextId.Value))
                    {
                        throw new UnauthorizedAccessException("Account is not the owner of the post");
                    }
                    
                    await _postService.UpdatePictureAsync(contextId.Value, imageUrl);
                    break;
                case "event":
                    
                    
                    break;
                case "menu-item":
                    break;
                default:
                    throw new ArgumentException("Invalid image context");
            }
        }
    }
}