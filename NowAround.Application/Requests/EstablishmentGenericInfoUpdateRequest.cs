namespace NowAround.Application.Requests;

public class EstablishmentGenericInfoUpdateRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int PriceCategory { get; set; }
    public ICollection<string> Categories { get; set; }
    public ICollection<string> Tags { get; set; }
}