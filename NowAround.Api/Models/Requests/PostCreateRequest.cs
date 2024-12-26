namespace NowAround.Api.Models.Requests;

public class PostCreateRequest
{
    public required string Headline { get; set; }
    public required string Body { get; set; }
}