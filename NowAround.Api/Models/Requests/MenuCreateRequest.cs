using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Models.Requests;

public class MenuCreateRequest
{
    [Required]
    public string Name { get; set; }
    public ICollection<MenuItemCreateRequest> MenuItems { get; set; }
}

public class MenuItemCreateRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public required string Description { get; set; }
    [Required]
    [Range(0, 10000)]
    public required double Price { get; set; }
}