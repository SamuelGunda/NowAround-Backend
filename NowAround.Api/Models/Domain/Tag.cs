using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class Tag : BaseEntity
{
    [MaxLength(32)]
    public required string Name { get; set; }
    
    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public virtual ICollection<EstablishmentTag> EstablishmentTags { get; } = new List<EstablishmentTag>();
}