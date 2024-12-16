using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class Tag : BaseEntity
{
    [MaxLength(32)]
    public required string Name { get; set; }
    public virtual ICollection<Establishment> Establishments { get; } = new List<Establishment>();
}