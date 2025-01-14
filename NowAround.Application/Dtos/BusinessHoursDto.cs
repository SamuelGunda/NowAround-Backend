namespace NowAround.Application.Dtos;

public sealed record BusinessHoursDto(
    string Monday,
    string Tuesday,
    string Wednesday,
    string Thursday,
    string Friday,
    string Saturday,
    string Sunday,
    ICollection<BusinessHoursExceptionsDto> BusinessHoursExceptions);