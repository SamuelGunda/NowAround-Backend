using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NowAround.Application.Common.Exceptions;
using NowAround.Application.Dtos;
using NowAround.Application.Mapping;
using NowAround.Application.Requests;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;

namespace NowAround.Application.Services;

public class PostService : IPostService
{
    private readonly ILogger<PostService> _logger;
    private readonly IEstablishmentService _establishmentService;
    private readonly IUserService _userService;
    private readonly IPostRepository _postRepository;
    private readonly IStorageService _storageService;
    
    public PostService(
        ILogger<PostService> logger, 
        IEstablishmentService establishmentService,
        IUserService userService,
        IPostRepository postRepository, 
        IStorageService storageService)
    {
        _logger = logger;
        _establishmentService = establishmentService;
        _userService = userService;
        _postRepository = postRepository;
        _storageService = storageService;
    }
    
    public async Task<PostDto> CreatePostAsync(PostCreateUpdateRequest postCreateRequest, string auth0Id)
    {
        var establishment = await _establishmentService.GetEstablishmentByAuth0IdAsync(auth0Id);
        
        var postEntity = new Post
        {
            Headline = postCreateRequest.Headline,
            Body = postCreateRequest.Body,
            EstablishmentId = establishment.Id
        };
        
        await _postRepository.CreateAsync(postEntity); 
        
        if (postCreateRequest.Picture is not null)
        {
            postEntity.PictureUrl = await _storageService.UploadPictureAsync(postCreateRequest.Picture, "Establishment", auth0Id, $"post/{postEntity.Id}");
            await _postRepository.UpdateAsync(postEntity);
        }
        
        return postEntity.ToDto();
    }

    public async Task<Post> GetPostAsync(int postId, bool tracked = false)
    {
        var post = await _postRepository.GetAsync
        (
            p => p.Id == postId, 
            tracked, 
            query => query.Include(q => q.Establishment), 
            query => query.Include(p => p.Likes)
        );
        
        return post;
    }
    public async Task ReactToPostAsync(int postId, string auth0Id)
    {
        var postEntity = await _postRepository.GetAsync(
            p => p.Id == postId, 
            true, 
            query => query.Include(p => p.Likes));
        
        if (postEntity.Likes.Any(l => l.Auth0Id == auth0Id))
        {
            postEntity.Likes.Remove(postEntity.Likes.First(l => l.Auth0Id == auth0Id));
        }
        else
        {
            postEntity.Likes.Add(await _userService.GetUserByAuth0IdAsync(auth0Id));
        }
        
        await _postRepository.UpdateAsync(postEntity);
    }

    public Task<PostDto> UpdatePostAsync(int postId, string auth0Id, PostCreateUpdateRequest postCreateUpdateRequest)
    {
        throw new NotImplementedException();
    }

    public async Task UpdatePictureAsync(int postId, string pictureUrl)
    {
        var post = await GetPostAsync(postId, true);
        
        post.PictureUrl = pictureUrl;
        
        await _postRepository.UpdateAsync(post);
    }

    public async Task DeletePostAsync(string auth0Id, int postId)
    {
        var post = await GetPostAsync(postId);
        
        if (auth0Id != post.Establishment.Auth0Id)
        {
            _logger.LogWarning("User {Auth0Id} is not the owner of post {PostId}", auth0Id, postId);
            throw new UnauthorizedAccessException("User is not the owner of the post");
        }
        
        await _postRepository.DeleteAsync(postId);
        
        if (post.PictureUrl is not null)
        {
            await _storageService.DeleteAsync("Establishment", auth0Id, $"post/{postId}");
        }
    }
}