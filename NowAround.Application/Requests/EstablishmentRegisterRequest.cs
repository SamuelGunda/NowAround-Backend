using System.ComponentModel.DataAnnotations;

namespace NowAround.Application.Requests;

public class EstablishmentRegisterRequest
{
    [Required]
    public required EstablishmentInfo EstablishmentInfo { get; set; }
    [Required]
    public required EstablishmentOwnerInfo EstablishmentOwnerInfo { get; set; }
}