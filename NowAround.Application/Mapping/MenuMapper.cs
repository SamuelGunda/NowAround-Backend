using NowAround.Application.Dtos;
using NowAround.Domain.Models;

namespace NowAround.Application.Mapping;

public static class MenuMapper
{
    public static MenuDto ToDto(this Menu menuEntity) {
        return new MenuDto(
            Id: menuEntity.Id, 
            Name: menuEntity.Name, 
            MenuItems: menuEntity.MenuItems.Select(x => x.ToDto()).ToList());
        }
}