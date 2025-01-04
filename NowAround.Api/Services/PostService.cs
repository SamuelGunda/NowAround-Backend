using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Requests;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Services;

public class PostService : IPostService
{
    private readonly ILogger<PostService> _logger;
    private readonly IEstablishmentService _establishmentService;
    private readonly IPostRepository _postRepository;
    
    public PostService(ILogger<PostService> logger ,IEstablishmentService establishmentService, IPostRepository postRepository)
    {
        _logger = logger;
        _establishmentService = establishmentService;
        _postRepository = postRepository;
    }
    
    public async Task<int> CreatePostAsync(PostCreateRequest postCreateRequest, string auth0Id)
    {
        var establishment = await _establishmentService.GetEstablishmentByAuth0Id(auth0Id);
        
        var post = new Post
        {
            Headline = postCreateRequest.Headline,
            Body = postCreateRequest.Body,
            EstablishmentId = establishment.Id
        };
        
        await _postRepository.CreateAsync(post);
        
        return post.Id;
    }

    public async Task<bool> CheckPostOwnershipByAuth0IdAsync(string auth0Id, int postId)
    {
        var establishment = await _establishmentService.GetEstablishmentByAuth0Id(auth0Id);
        
        var post = await GetPostAsync(postId);
        
        return establishment.Id == post.EstablishmentId;
    }

    public async Task<Post> GetPostAsync(int postId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        
        if (post == null)
        {
            _logger.LogWarning("Post with ID: {PostId} not found", postId);
            throw new EntityNotFoundException("Post", "ID", postId.ToString());
        }
        
        return post;
    }

    public async Task UpdatePictureAsync(int postId, string pictureUrl)
    {
        var post = await GetPostAsync(postId);
        
        post.PictureUrl = pictureUrl;
        
        await _postRepository.UpdateAsync(post);
    }

    public async Task DeletePostAsync(int postId)
    {
        var result = await _postRepository.DeleteAsync(postId);
        
        if (!result)
        {
            throw new EntityNotFoundException("Post", "ID", postId.ToString());
        }
    }
}