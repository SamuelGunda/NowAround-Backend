using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Models.Requests;

public class ReviewCreateRequest
{
    [Required]
    public required string EstablishmentAuth0Id { get; set; }
    [Required]
    public required int Rating { get; set; }
    public string? Body { get; set; }
}