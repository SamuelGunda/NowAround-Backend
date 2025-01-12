using System.ComponentModel.DataAnnotations;
using NowAround.Api.Utilities;

namespace NowAround.Api.Models.Requests;

public class PostCreateUpdateRequest
{
    [Required]
    public required string Headline { get; set; }
    [Required]
    public required string Body { get; set; }
    [ContentType([ "image/jpeg", "image/png", "image/gif", "image/webp"])]
    public IFormFile? Picture { get; set; }
}