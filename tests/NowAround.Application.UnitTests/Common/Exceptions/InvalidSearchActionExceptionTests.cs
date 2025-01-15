using FluentAssertions;
using NowAround.Application.Common.Exceptions;

namespace NowAround.Application.UnitTests.Common.Exceptions;

public class InvalidSearchActionExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "Invalid search action performed.";

        // Act
        var exception = new InvalidSearchActionException(expectedMessage);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(expectedMessage);
    }
}