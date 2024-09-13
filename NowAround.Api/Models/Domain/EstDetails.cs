namespace NowAround.Api.Models.Domain;

public class EstDetails
{
    public int Id { get; set; }
    
    public int AccountId { get; set; }
    public virtual Account Account { get; set; }
    
    public required string Name { get; set; }
    public string Description { get; set; }
    public required string Address { get; set; }
    public string Website { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public virtual ICollection<EstDetailsCategory> EstDetailsCategories { get; set; } = new List<EstDetailsCategory>();
    public virtual ICollection<EstDetailsTag> EstDetailsTags { get; set; } = new List<EstDetailsTag>();
}