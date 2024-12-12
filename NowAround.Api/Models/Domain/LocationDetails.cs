using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

//TODO: May use in the future
public class LocationDetails : BaseEntity
{
    [MaxLength(32)]
    public required string City { get; set; }
    [MaxLength(64)]
    public required string Address { get; set; }
    
    public int BusinessHoursId { get; set; }
    public virtual BusinessHours BusinessHours { get; set; }
    
    public int EstablishmentId { get; set; }
    public Establishment Establishment { get; set; }
}