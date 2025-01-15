using System.ComponentModel.DataAnnotations;
using NowAround.Domain.Common;

namespace NowAround.Domain.Models;

public class Category : BaseEntity
{
    [MaxLength(32)]
    public required string Name { get; set; }
    
    public ICollection<Establishment> Establishments { get; set; } = new List<Establishment>();
}