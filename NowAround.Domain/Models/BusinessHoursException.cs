
using NowAround.Domain.Common;

namespace NowAround.Domain.Models;

public class BusinessHoursException : BaseEntity
{
    public DateOnly Date { get; set; }
    public string Status { get; set; }
    
    public int BusinessHoursId { get; set; }
    public BusinessHours BusinessHours { get; set; }
}