using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Requests;

public class EventCreateRequest
{
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Body { get; set; }
    public double? Price { get; set; }
    [Required]
    public required string Address { get; set; }
    [Required]
    public required string City { get; set; }
    [Required]
    public required string MaxParticipants { get; set; }
    [Required]
    public required string EventCategory { get; set; }
    public IFormFile? Picture { get; set; }
    [Required]
    public required DateTime Start { get; set; }
    [Required]
    public required DateTime End { get; set; }

}