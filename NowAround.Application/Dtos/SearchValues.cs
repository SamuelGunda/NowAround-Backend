using NowAround.Application.Common.Exceptions;
using NowAround.Domain.Enum;

namespace NowAround.Application.Dtos;

public class SearchValues
{
    public string? Name { get; set; }
    public int? PriceCategory { get; set; }
    public string? CategoryName { get; set; }
    public List<string>? TagNames { get; set; }
    public required MapBounds MapBounds { get; set; }
    
    public void ValidateProperties()
    {
        if (PriceCategory.HasValue && !System.Enum.IsDefined(typeof(PriceCategory), PriceCategory.Value))
        {
            throw new ArgumentException($"PriceCategory value {PriceCategory} is not valid");
        }
        
        if (!string.IsNullOrWhiteSpace(Name) && Name.Length < 3)
        {
            throw new ArgumentException("Name must be at least 3 characters long");
        }
        
        if (string.IsNullOrWhiteSpace(Name) && !PriceCategory.HasValue && string.IsNullOrWhiteSpace(CategoryName) && (TagNames == null || TagNames.Count == 0) && !MapBounds.ValidateProperties())
        {
            throw new InvalidSearchActionException("At least one search value must be provided");
        }
    }
}