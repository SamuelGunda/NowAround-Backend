using FluentAssertions;
using NowAround.Application.Common.Exceptions;

namespace NowAround.Application.UnitTests.Common.Exceptions;

public class EstablishmentAlreadyExistsExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessageCorrectly()
    {
        // Arrange
        const string name = "Test Establishment";
        const string expectedMessage = $"The establishment with name: {name} already exists";

        // Act
        var exception = new EstablishmentAlreadyExistsException(name);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(expectedMessage);
    }
}