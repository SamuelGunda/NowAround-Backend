namespace NowAround.Application.Dtos;

public sealed record MenuDto(int Id, string Name, ICollection<MenuItemDto> MenuItems);