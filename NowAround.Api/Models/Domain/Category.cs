using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Models.Domain;

public class Category
{
    public int Id { get; init; }
    
    [MaxLength(32)]
    public required string Name { get; init; }

    public virtual ICollection<Tag> Tags { get; } = new List<Tag>();
    public virtual ICollection<EstablishmentCategory> EstablishmentCategories { get; } = new List<EstablishmentCategory>();
}