using NowAround.Application.Dtos;
using NowAround.Domain.Models;

namespace NowAround.Application.Mapping;

public static class PostMapper
{
    public static PostDto ToDto(this Post postEntity)
    {
        ArgumentNullException.ThrowIfNull(postEntity, nameof(postEntity));
        
        return new PostDto(
            Id: postEntity.Id,
            CreatorAuth0Id:postEntity.Establishment?.Auth0Id ?? null,
            Headline: postEntity.Headline,
            Body: postEntity.Body,
            PictureUrl: postEntity.PictureUrl,
            CreatedAt: postEntity.CreatedAt,
            Likes: postEntity.Likes.Select(x => x.Auth0Id).ToList()
        );
    }
}