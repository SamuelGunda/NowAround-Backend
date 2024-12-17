namespace NowAround.Api.Services.Interfaces;

public interface IStorageService
{
    Task UploadImageAsync(IFormFile file, string role, string auth0Id, string imageContext, string contextId);
}