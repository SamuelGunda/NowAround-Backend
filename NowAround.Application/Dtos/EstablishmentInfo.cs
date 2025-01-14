namespace NowAround.Application.Dtos;

public class EstablishmentInfo
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public int PriceCategory { get; set; }
    public ICollection<string> Category { get; set; }
    public ICollection<string> Tags { get; set; }

    public void ValidateProperties()
    {
        if (string.IsNullOrEmpty(Name)
            || string.IsNullOrEmpty(Address)
            || string.IsNullOrEmpty(City)
            || string.IsNullOrEmpty(PostalCode)
            || PriceCategory < 0 || PriceCategory >= 3
            || Category == null || Category.Count == 0
            || Tags == null || Tags.Count == 0)
        {
            throw new ArgumentNullException();
        }
    }
}