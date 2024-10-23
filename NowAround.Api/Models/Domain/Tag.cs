using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Models.Domain;

public class Tag
{
    public int Id { get; init; }
    
    [MaxLength(32)]
    public required string Name { get; init; }
    
    public int? CategoryId { get; init; }
    public virtual Category? Category { get; init; }
    public virtual ICollection<EstablishmentTag> EstablishmentTags { get; } = new List<EstablishmentTag>();
}