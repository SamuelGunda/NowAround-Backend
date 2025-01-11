using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Models.Requests;

public class PostCreateRequest
{
    [Required]
    public required string Headline { get; set; }
    [Required]
    public required string Body { get; set; }
    public IFormFile? Picture { get; set; }
}