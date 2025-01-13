using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Models.Requests;

public class EstablishmentLocationInfoUpdateRequest
{
    public double Long { get; set; }
    public double Lat { get; set; }
    public BusinessHoursDto BusinessHours { get; set; }
}