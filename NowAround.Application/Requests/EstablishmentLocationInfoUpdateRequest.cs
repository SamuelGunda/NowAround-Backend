using NowAround.Application.Dtos;

namespace NowAround.Application.Requests;

public class EstablishmentLocationInfoUpdateRequest
{
    public double Long { get; set; }
    public double Lat { get; set; }
    public BusinessHoursDto BusinessHours { get; set; }
}