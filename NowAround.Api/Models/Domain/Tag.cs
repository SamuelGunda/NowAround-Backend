namespace NowAround.Api.Models.Domain;

public class Tag
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    public required string SkName { get; set; }
    public string Icon { get; set; }
    
    public virtual ICollection<EstablishmentTag> EstablishmentTags { get; } = new List<EstablishmentTag>();
}