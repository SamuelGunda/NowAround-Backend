using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class Cuisine : BaseEntity
{
    [MaxLength(32)]
    public string Name { get; set; }
    
    public ICollection<Establishment> Establishments { get; set; } = new List<Establishment>();
}