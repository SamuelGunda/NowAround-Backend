using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class Post : BaseEntity
{
    [MaxLength(64)]
    public required string Headline { get; set; }
    [MaxLength(512)]
    public required string Body { get; set; }
    [MaxLength(256)]
    public string? PictureUrl { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    public ICollection<User> Likes { get; init; } = new List<User>();
    public int EstablishmentId { get; init; }
    public Establishment Establishment { get; init; }
    
    public PostDto ToDto()
    {
        var establishmentAuth0Id = Establishment?.Auth0Id ?? null;
        var postLikesAuth0Ids = Likes.Select(l => l.Auth0Id).ToList();

        return new PostDto(Id, establishmentAuth0Id, Headline, Body, PictureUrl, CreatedAt, postLikesAuth0Ids);
    }
}