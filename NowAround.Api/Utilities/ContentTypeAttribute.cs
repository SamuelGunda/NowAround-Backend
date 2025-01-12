using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Utilities;

public class ContentTypeAttribute(string[] validContentTypes) : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is not IFormFile file)
        {
            return new ValidationResult("Invalid file type");
        }

        if (!validContentTypes.Contains(file.ContentType))
        {
            return new ValidationResult($"Invalid content type. Allowed types are {string.Join(", ", validContentTypes)}");
        }

        return ValidationResult.Success;
    }
    
}