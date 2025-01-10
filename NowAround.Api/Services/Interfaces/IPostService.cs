using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Requests;

namespace NowAround.Api.Services.Interfaces;

public interface IPostService
{
    Task<PostDto> CreatePostAsync(PostCreateRequest postCreateRequest, string auth0Id);
    Task<Post> GetPostAsync(int postId, bool tracked = false);
    Task UpdatePictureAsync(int postId, string pictureUrl);
    Task DeletePostAsync(string auth0Id, int postId);
}