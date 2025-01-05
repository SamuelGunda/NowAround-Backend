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
    private readonly IStorageService _storageService;
    
    public PostService(ILogger<PostService> logger ,IEstablishmentService establishmentService, IPostRepository postRepository, IStorageService storageService)
    {
        _logger = logger;
        _establishmentService = establishmentService;
        _postRepository = postRepository;
        _storageService = storageService;
    }
    
    public async Task<int> CreatePostAsync(PostCreateRequest postCreateRequest, string auth0Id)
    {
        if (postCreateRequest.Image != null)
        {
            var pictureType = postCreateRequest.Image.ContentType;
            _storageService.CheckValidImageType(pictureType);
        }
        
        var establishment = await _establishmentService.GetEstablishmentByAuth0Id(auth0Id);
        
        var post = new Post
        {
            Headline = postCreateRequest.Headline,
            Body = postCreateRequest.Body,
            EstablishmentId = establishment.Id
        };
        
        var id = await _postRepository.CreateAsync(post);
        
        if (postCreateRequest.Image != null)
        {
            var pictureUrl = await _storageService.UploadImageAsync(postCreateRequest.Image, "Establishment", auth0Id, "post", id);
            await UpdatePictureAsync(id, pictureUrl);
        }
        
        return id;
    }

    public async Task<bool> CheckPostOwnershipByAuth0IdAsync(string auth0Id, int postId)
    {
        var post = await GetPostAsync(postId);
        
        return auth0Id == post.Establishment.Auth0Id;
    }

    public async Task<Post> GetPostAsync(int postId, bool tracked = false)
    {

        var post = await _postRepository.GetAsync(p => p.Id == postId, 
            tracked, 
            p => p.Establishment);
        
        if (post == null)
        {
            _logger.LogWarning("Post with ID: {PostId} not found", postId);
            throw new EntityNotFoundException("Post", "ID", postId.ToString());
        }
        
        return post;
    }

    public async Task UpdatePictureAsync(int postId, string pictureUrl)
    {
        var post = await GetPostAsync(postId, true);
        
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