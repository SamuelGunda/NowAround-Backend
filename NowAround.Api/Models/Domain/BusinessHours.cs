using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class BusinessHours : BaseEntity
{
    [MaxLength(48)]
    public string Monday { get; set; } = "Not Set";
    [MaxLength(48)]
    public string Tuesday { get; set; } = "Not Set";
    [MaxLength(48)]
    public string Wednesday { get; set; }  = "Not Set";
    [MaxLength(48)]
    public string Thursday { get; set; } = "Not Set";
    [MaxLength(48)]
    public string Friday { get; set; } = "Not Set";
    [MaxLength(48)]
    public string Saturday { get; set; } = "Not Set";
    [MaxLength(48)]
    public string Sunday { get; set; } = "Not Set";
    
    public int EstablishmentId { get; set; }
    public Establishment Establishment { get; set; }
    
    public ICollection<BusinessHoursException> BusinessHoursExceptions { get; set; } = new List<BusinessHoursException>();
}