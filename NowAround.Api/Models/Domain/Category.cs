namespace NowAround.Api.Models.Domain;

public class Category
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    public required string SkName { get; set; }
    public string Icon { get; set; }
    
    public virtual ICollection<EstablishmentCategory> EstablishmentCategories { get; } = new List<EstablishmentCategory>();
}