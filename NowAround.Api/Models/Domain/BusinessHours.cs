using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class BusinessHours : BaseEntity
{
    public string Monday { get; set; }
    public string Tuesday { get; set; }
    public string Wednesday { get; set; }
    public string Thursday { get; set; }
    public string Friday { get; set; }
    public string Saturday { get; set; }
    public string Sunday { get; set; }
    
    public int EstablishmentId { get; set; }
    public Establishment Establishment { get; set; }
    
    public ICollection<BusinessHoursException> BusinessHoursExceptions { get; set; }
}