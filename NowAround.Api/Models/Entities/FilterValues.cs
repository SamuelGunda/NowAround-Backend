using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Entities;

public class FilterValues
{
    public string? Name { get; set; }
    public int? PriceCategory { get; set; }
    public string? CategoryName { get; set; }
    public List<string>? TagNames { get; set; }
    public required MapBounds MapBounds { get; set; }
    
    public bool ValidateProperties()
    {
        var filterValueProvided = true;
        
        if (PriceCategory.HasValue && !System.Enum.IsDefined(typeof(PriceCategory), PriceCategory.Value))
        {
            throw new ArgumentException("Invalid price category");
        }
        
        if (!string.IsNullOrWhiteSpace(Name) && Name.Length < 3)
        {
            throw new ArgumentException("Name is too short");
        }
        
        if (string.IsNullOrWhiteSpace(Name) && !PriceCategory.HasValue && string.IsNullOrWhiteSpace(CategoryName) && (TagNames == null || TagNames.Count == 0) && !MapBounds.ValidateProperties())
        {
            filterValueProvided = false;
        }

        return filterValueProvided;
    }
}