using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Requests;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Services;

public class PostService : IPostService
{
    private readonly IEstablishmentService _establishmentService;
    private readonly IPostRepository _postRepository;
    
    public PostService(IEstablishmentService establishmentService, IPostRepository postRepository)
    {
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
        Console.WriteLine(establishment.Id);
        Console.WriteLine(auth0Id);
        
        var post = await _postRepository.GetByIdAsync(postId);
        
        if (post == null)
        {
            throw new EntityNotFoundException("Post", "ID", postId.ToString());
        }
        
        return establishment.Id == post.EstablishmentId;
    }

    public Task<Post> GetPostAsync(int postId)
    {
        throw new NotImplementedException();
    }

    public async Task UpdatePictureAsync(int postId, string imageUrl)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        
        if (post == null)
        {
            throw new EntityNotFoundException("Post", "ID", postId.ToString());
        }
        
        post.ImageUrl = imageUrl;
        
        await _postRepository.UpdateAsync(post);
    }
}