using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Models.Requests;

public class MenuCreateRequest
{
    public string Name { get; set; }
    public ICollection<MenuItemCreateRequest> MenuItems { get; set; }
}

public class MenuItemCreateRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    [Range(0, 10000)]
    public required double Price { get; set; }
}