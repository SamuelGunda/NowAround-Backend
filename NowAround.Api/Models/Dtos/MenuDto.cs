namespace NowAround.Api.Models.Dtos;

public sealed record MenuDto(int Id, string CreatorAuth0Id, string Name, ICollection<MenuItemDto> MenuItems);