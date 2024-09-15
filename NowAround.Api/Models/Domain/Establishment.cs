namespace NowAround.Api.Models.Domain;

public class Establishment
{
    public int Id { get; set; }
    
    public required int AccountId { get; set; }
    public virtual required  Account Account { get; set; }
    
    public required string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public string Website { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public virtual ICollection<EstablishmentCategory> EstDetailsCategories { get; set; } = new List<EstablishmentCategory>();
    public virtual ICollection<EstablishmentTag> EstDetailsTags { get; set; } = new List<EstablishmentTag>();
}