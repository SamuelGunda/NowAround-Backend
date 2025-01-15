using System.ComponentModel.DataAnnotations;
using NowAround.Domain.Enum;

namespace NowAround.Application.Requests;

public class EstablishmentGenericInfoUpdateRequest
{
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Description { get; set; }
    [Required]
    [EnumDataType(typeof(PriceCategory))]
    public required int PriceCategory { get; set; }
    [Required]
    public required ICollection<string> Categories { get; set; }
    [Required]
    public required ICollection<string> Tags { get; set; }
}