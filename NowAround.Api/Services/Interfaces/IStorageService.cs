﻿namespace NowAround.Api.Services.Interfaces;

public interface IStorageService
{
    Task<string> UploadPictureAsync(IFormFile file, string role, string auth0Id, string imageContext, int? contextId);
    Task DeleteAsync(string role, string auth0Id, string imageContext = "");
    void CheckPictureType(string contentType);
}