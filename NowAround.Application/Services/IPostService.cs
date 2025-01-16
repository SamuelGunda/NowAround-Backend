using NowAround.Application.Dtos;
using NowAround.Application.Requests;
using NowAround.Domain.Models;

namespace NowAround.Application.Services;

public interface IPostService
{
    Task<PostDto> CreatePostAsync(PostCreateUpdateRequest postCreateRequest, string auth0Id);
    Task<Post> GetPostAsync(int postId, bool tracked = false);
    Task ReactToPostAsync(int postId, string auth0Id);
    Task DeletePostAsync(string auth0Id, int postId);
}