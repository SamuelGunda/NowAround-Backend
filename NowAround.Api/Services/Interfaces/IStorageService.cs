namespace NowAround.Api.Services.Interfaces;

public interface IStorageService
{
    Task<string> UploadPictureAsync(IFormFile file, string role, string auth0Id, string imageContext, int? contextId);
    Task DeletePictureAsync(string role, string auth0Id, string imageContext, int? contextId);
    Task DeleteAccountFolderAsync(string role, string auth0Id);
    void CheckPictureType(string contentType);
}