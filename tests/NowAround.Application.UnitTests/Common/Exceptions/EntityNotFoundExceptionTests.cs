using FluentAssertions;
using NowAround.Application.Common.Exceptions;

namespace NowAround.Application.UnitTests.Common.Exceptions;

public class EntityNotFoundExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessageCorrectly()
    {
        // Arrange
        const string entity = "User";
        const string property = "Id";
        const string value = "123";
        const string expectedMessage = $"The {entity} with {property}: {value} was not found";

        // Act
        var exception = new EntityNotFoundException(entity, property, value);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(expectedMessage);
    }
}