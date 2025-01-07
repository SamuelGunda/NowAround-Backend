using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class Menu : BaseEntity
{
    public required string Name { get; set; }
    
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    
    public int EstablishmentId { get; set; }
    public virtual Establishment Establishment { get; set; }
}