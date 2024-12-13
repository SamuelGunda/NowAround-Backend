using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class Category : BaseEntity
{
    [MaxLength(32)]
    public required string Name { get; set; }
    public virtual ICollection<EstablishmentCategory> EstablishmentCategories { get; set; } = new List<EstablishmentCategory>();
}