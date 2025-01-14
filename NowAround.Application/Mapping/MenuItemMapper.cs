using NowAround.Application.Dtos;
using NowAround.Domain.Models;

namespace NowAround.Application.Mapping;

public static class MenuItemMapper
{
    public static MenuItemDto ToDto(this MenuItem menuItemEntity)
    {
        return new MenuItemDto(
            Id: menuItemEntity.Id,
            Name: menuItemEntity.Name,
            PictureUrl: menuItemEntity.PictureUrl,
            Description: menuItemEntity.Description,
            Price: menuItemEntity.Price);
    }
}