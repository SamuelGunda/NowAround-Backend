namespace NowAround.Api.Models.Domain;

public class EstDetailsCategory
{
    public int EstDetailsId { get; set; }
    public EstDetails EstDetails { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}