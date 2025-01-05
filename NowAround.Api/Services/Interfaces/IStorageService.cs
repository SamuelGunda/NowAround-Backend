namespace NowAround.Api.Services.Interfaces;

public interface IStorageService
{
    Task<string> UploadImageAsync(IFormFile file, string role, string auth0Id, string imageContext, int? contextId);

    void CheckValidImageType(string contentType);
}