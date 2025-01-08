using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Models.Requests;

public class EventCreateRequest
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Body { get; set; }
    public double? Price { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public double Latitude { get; set; }
    [Required]
    public double Longitude { get; set; }
    [Required]
    public int MaxParticipants { get; set; }
    public IFormFile? Picture { get; set; }
    [Required]
    public DateTime Start { get; set; }
    [Required]
    public DateTime End { get; set; }
    [Required]
    public int EventCategory { get; set; }
}