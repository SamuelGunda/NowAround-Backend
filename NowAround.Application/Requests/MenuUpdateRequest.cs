using System.ComponentModel.DataAnnotations;

namespace NowAround.Application.Requests;

public class MenuUpdateRequest
{
    [Required]
    public required int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    public ICollection<MenuItemUpdateRequest> MenuItems { get; set; }
}

public class MenuItemUpdateRequest
{
    public int? Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Description { get; set; }
    [Required]
    [Range(0, 10000)]
    public required double Price { get; set; }
}