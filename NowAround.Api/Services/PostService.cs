using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Requests;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Services;

public class PostService : IPostService
{
    private readonly IEstablishmentService _establishmentService;
    private readonly IPostRepository _psotRepository;
    
    public PostService(IEstablishmentService establishmentService, IPostRepository postRepository)
    {
        _establishmentService = establishmentService;
        _psotRepository = postRepository;
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
        
        // Save post to database
        await _psotRepository.CreateAsync(post);
        
        return post.Id;
    }

    public Task<Post> GetPostAsync(int postId)
    {
        throw new NotImplementedException();
    }
}