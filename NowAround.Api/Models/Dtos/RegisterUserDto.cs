using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Models.Dtos;

public class RegisterUserDto
{
    [Required]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    public string FullName { get; set; }
}