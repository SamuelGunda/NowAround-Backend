namespace NowAround.Api.Models.Domain;

public class EstDetailsTag
{
    public int EstDetailsId { get; set; }
    public EstDetails EstDetails { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}