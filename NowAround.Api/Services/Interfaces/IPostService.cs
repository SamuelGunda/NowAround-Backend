using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Requests;

namespace NowAround.Api.Services.Interfaces;

public interface IPostService
{
    Task<int> CreatePostAsync(PostCreateRequest postCreateRequest, string auth0Id);
    Task<bool> CheckPostOwnershipByAuth0IdAsync(string auth0Id, int postId);
    Task<Post> GetPostAsync(int postId, bool tracked = false);
    Task UpdatePictureAsync(int postId, string pictureUrl);
    Task DeletePostAsync(int postId);
}