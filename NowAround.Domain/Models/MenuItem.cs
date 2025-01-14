using System.ComponentModel.DataAnnotations;
using NowAround.Domain.Common;

namespace NowAround.Domain.Models;

public class MenuItem : BaseEntity
{
    [MaxLength(64)]
    public required string Name { get; set; }
    [MaxLength(512)]
    public required string Description { get; set; }
    [Range(0, 10000)]
    public required double Price { get; set; }
    [MaxLength(256)]
    public string? PictureUrl { get; set; }
    
    public int MenuId { get; set; }
    public  Menu Menu { get; set; }
}