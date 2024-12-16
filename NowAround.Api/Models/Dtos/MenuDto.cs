namespace NowAround.Api.Models.Dtos;

public sealed record MenuDto(string Name, ICollection<MenuItemDto> MenuItems);