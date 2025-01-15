
using System.ComponentModel.DataAnnotations;
using NowAround.Domain.Enum;

namespace NowAround.Application.Requests;

public class EstablishmentInfo
{
    [Required]
    public required string Name { get; init; }
    [Required]
    public required string Address { get; init; }
    [Required]
    public required string PostalCode { get; init; }
    [Required]
    public required string City { get; init; }
    [Required]
    [EnumDataType(typeof(PriceCategory))]
    public required int? PriceCategory { get; init; }
    
    [MinLength(1, ErrorMessage = "At least one Category is required")]
    public required ICollection<string> Category { get; init; }
    [MinLength(1, ErrorMessage = "At least one Tag is required")]
    public required ICollection<string> Tags { get; init; }
}