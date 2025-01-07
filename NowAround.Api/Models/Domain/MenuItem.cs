using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class MenuItem : BaseEntity
{
    [MaxLength(64)]
    public required string Name { get; set; }
    [MaxLength(512)]
    public required string Description { get; set; }
    [Range(0, 10000)]
    public required double Price { get; set; }
    [MaxLength(256)]
    public string? PictureUrl { get; set; } = null;
    
    public int MenuId { get; set; }
    public virtual Menu Menu { get; set; }
}