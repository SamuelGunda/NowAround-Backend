namespace NowAround.Api.Services.Interfaces;

public interface IStorageService
{
    Task<string> UploadPictureAsync(IFormFile file, string role, string auth0Id, string imageContext, int? contextId);

    void CheckPictureType(string contentType);
}