namespace NowAround.Api.Models.Dtos;

public sealed record PostDto(int Id, string CreatorAuth0Id, string Headline, string Body, string PictureUrl, DateTime CreatedAt, List<string> Likes);