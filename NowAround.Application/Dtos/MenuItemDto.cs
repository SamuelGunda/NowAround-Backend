namespace NowAround.Application.Dtos;

public sealed record MenuItemDto(int Id, string Name, string? PictureUrl, string Description, double Price);