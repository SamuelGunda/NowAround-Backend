using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Requests;

public class EventCreateRequest
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Body { get; set; }
    public double? Price { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string PostalCode { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string MaxParticipants { get; set; }
    [Required]
    public string EventCategory { get; set; }
    public IFormFile? Picture { get; set; }
    [Required]
    public DateTime Start { get; set; }
    [Required]
    public DateTime End { get; set; }

}