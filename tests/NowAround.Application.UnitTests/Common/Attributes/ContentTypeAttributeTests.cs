using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NowAround.Application.Common.Attributes;

namespace NowAround.Application.UnitTests.Common.Attributes;

public class ContentTypeAttributeTests
{
    [Fact]
    public void ValidContentType_ReturnsSuccess()
    {
        // Arrange
        var validTypes = new[] { "image/jpeg", "image/png" };
        var attribute = new ContentTypeAttribute(validTypes);

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");

        // Act
        var result = attribute.GetValidationResult(mockFile.Object, new ValidationContext(mockFile.Object));

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void NullContentType_ReturnsSuccess()
    {
        // Arrange
        var validTypes = new[] { "image/jpeg", "image/png" };
        var attribute = new ContentTypeAttribute(validTypes);

        var mockFile = new Mock<IFormFile>();

        // Act
        var result = attribute.GetValidationResult(null, new ValidationContext(mockFile.Object));

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void InvalidContentType_ReturnsError()
    {
        // Arrange
        var validTypes = new[] { "image/jpeg", "image/png" };
        var attribute = new ContentTypeAttribute(validTypes);

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.ContentType).Returns("application/pdf");

        // Act
        var result = attribute.GetValidationResult(mockFile.Object, new ValidationContext(mockFile.Object));

        // Assert
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void InvalidFile_ReturnsError()
    {
        // Arrange
        var validTypes = new[] { "image/jpeg", "image/png" };
        var attribute = new ContentTypeAttribute(validTypes);

        var mockFile = new Mock<IFormFile>();

        // Act
        var result = attribute.GetValidationResult(mockFile.Object, new ValidationContext(mockFile.Object));

        // Assert
        result.Should().NotBe(ValidationResult.Success);
    }

}