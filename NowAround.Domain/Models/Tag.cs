using System.ComponentModel.DataAnnotations;
using NowAround.Domain.Common;

namespace NowAround.Domain.Models;

public class Tag : BaseEntity
{
    [MaxLength(32)]
    public required string Name { get; set; }
    public virtual ICollection<Establishment> Establishments { get; } = new List<Establishment>();
}