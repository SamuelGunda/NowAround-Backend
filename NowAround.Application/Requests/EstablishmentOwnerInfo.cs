using System.ComponentModel.DataAnnotations;

namespace NowAround.Application.Requests;

public class EstablishmentOwnerInfo
{
    [Required]
    public required string FirstName { get; init; }
    [Required]
    public required string LastName { get; init; }
    [Required]
    [EmailAddress]
    public required string Email { get; init; }
}