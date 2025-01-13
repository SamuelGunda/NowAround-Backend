using NowAround.Api.Exceptions;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Entities;

public class SearchValues
{
    public string? Name { get; set; }
    public int? PriceCategory { get; set; }
    public string? CategoryName { get; set; }
    public List<string>? TagNames { get; set; }
    public required MapBounds MapBounds { get; set; }
    
    public bool ValidateProperties()
    {
        if (string.IsNullOrWhiteSpace(Name) && !PriceCategory.HasValue && string.IsNullOrWhiteSpace(CategoryName) && (TagNames == null || TagNames.Count == 0) && !MapBounds.ValidateProperties())
        {
            return false;
        }
        
        return true;
    }
}