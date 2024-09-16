using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Interfaces;

public interface IAuth0Service
{
    Task<string> RegisterUserAsync(RegisterUserDto registerUserDto);
    Task<string> LoginUserAsync(LoginUserDto loginUserDto);
}