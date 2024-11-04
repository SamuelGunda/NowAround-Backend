namespace NowAround.Api.Models.Requests;

public class CategoriesAndTags
{
    
    public ICollection<string>? CategoryNames { get; set; }
    public ICollection<string>? TagNames { get; set; }
}