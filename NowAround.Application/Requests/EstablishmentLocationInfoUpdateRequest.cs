using System.ComponentModel.DataAnnotations;
using NowAround.Application.Dtos;

namespace NowAround.Application.Requests;

public class EstablishmentLocationInfoUpdateRequest
{
    [Required]
    public required double Long { get; set; }
    [Required]
    public required double Lat { get; set; }
    public BusinessHoursDto BusinessHours { get; set; }
}