namespace NowAround.Application.Dtos;

public class SearchValues
{
    public string? Name { get; set; }
    public int? PriceCategory { get; set; }
    public string? CategoryName { get; set; }
    public List<string>? TagNames { get; set; }
    public required MapBounds MapBounds { get; set; }
}