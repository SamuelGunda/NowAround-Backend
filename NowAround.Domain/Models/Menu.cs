using NowAround.Domain.Common;

namespace NowAround.Domain.Models;

public class Menu : BaseEntity
{
    public required string Name { get; set; }
    
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    
    public int EstablishmentId { get; set; }
    public  Establishment Establishment { get; set; }
}