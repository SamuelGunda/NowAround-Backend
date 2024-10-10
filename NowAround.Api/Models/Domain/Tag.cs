namespace NowAround.Api.Models.Domain;

public class Tag
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    public required string SkName { get; set; }
    public string Icon { get; set; }
    
    
    public int? CategoryId { get; set; }
    
    public virtual Category? Category { get; set; }
    
    public virtual ICollection<EstablishmentTag> EstablishmentTags { get; } = new List<EstablishmentTag>();
}