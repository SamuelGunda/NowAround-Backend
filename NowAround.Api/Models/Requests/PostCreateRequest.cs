using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Models.Requests;

public class PostCreateRequest
{
    [Required]
    public string Headline { get; set; }
    [Required]
    public string Body { get; set; }
    public IFormFile? Picture { get; set; }
}