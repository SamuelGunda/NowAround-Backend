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
    private readonly IPostRepository _postRepository;
    private readonly IStorageService _storageService;
    
    public PostService(ILogger<PostService> logger ,IEstablishmentService establishmentService, IPostRepository postRepository, IStorageService storageService)
    {
        _logger = logger;
        _establishmentService = establishmentService;
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

    private async Task<bool> CheckPostOwnershipByAuth0IdAsync(string auth0Id, int postId)
    {
        var post = await GetPostAsync(postId);
        
        return auth0Id == post.Establishment.Auth0Id;
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
        
        if (post == null)
        {
            _logger.LogWarning("Post with ID: {PostId} not found", postId);
            throw new EntityNotFoundException("Post", "ID", postId.ToString());
        }
        
        return post;
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
        if (!await CheckPostOwnershipByAuth0IdAsync(auth0Id, postId))
        {
            throw new UnauthorizedAccessException("You are not the owner of this post");
        }
        
        await _postRepository.DeleteAsync(postId);
        
        await _storageService.DeleteAsync("Establishment", auth0Id, $"post/{postId}");
    }
}