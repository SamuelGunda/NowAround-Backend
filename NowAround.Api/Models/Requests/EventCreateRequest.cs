using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Enum;
using NowAround.Api.Utilities;

namespace NowAround.Api.Models.Requests;

public class EventCreateRequest
{
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Body { get; set; }
    [Required]
    public required string Price { get; set; }
    [Required]
    [RegularExpression("^(eur|usd|per|\\*)?$")]
    public required string EventPriceCategory { get; set; }
    [Required]
    public required string Address { get; set; }
    [Required]
    public required string City { get; set; }
    public string? MaxParticipants { get; set; }
    [Required]
    [EnumDataType(typeof(EventCategory))]
    public required string EventCategory { get; set; }
    [ContentType([ "image/jpeg", "image/png", "image/gif", "image/webp"])]
    public IFormFile? Picture { get; set; }
    [Required]
    public required DateTime Start { get; set; }
    [Required]
    public required DateTime End { get; set; }
}