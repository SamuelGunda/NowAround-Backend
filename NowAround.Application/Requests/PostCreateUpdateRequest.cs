using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using NowAround.Application.Common.Attributes;

namespace NowAround.Application.Requests;

public class PostCreateUpdateRequest
{
    [Required]
    public required string Headline { get; set; }
    [Required]
    public required string Body { get; set; }
    [ContentType([ "image/jpeg", "image/png", "image/gif", "image/webp"])]
    public IFormFile? Picture { get; set; }
}