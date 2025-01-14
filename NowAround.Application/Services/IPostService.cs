using NowAround.Application.Dtos;
using NowAround.Application.Requests;
using NowAround.Domain.Models;

namespace NowAround.Application.Services;

public interface IPostService
{
    Task<PostDto> CreatePostAsync(PostCreateUpdateRequest postCreateRequest, string auth0Id);
    Task<Post> GetPostAsync(int postId, bool tracked = false);
    Task ReactToPostAsync(int postId, string auth0Id);
    Task<PostDto> UpdatePostAsync(int postId, string auth0Id, PostCreateUpdateRequest postCreateUpdateRequest);
    Task UpdatePictureAsync(int postId, string pictureUrl);
    Task DeletePostAsync(string auth0Id, int postId);
}