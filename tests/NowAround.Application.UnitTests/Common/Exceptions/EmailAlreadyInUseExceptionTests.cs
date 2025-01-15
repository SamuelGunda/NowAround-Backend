using FluentAssertions;
using NowAround.Application.Common.Exceptions;

namespace NowAround.Application.UnitTests.Common.Exceptions;

public class EmailAlreadyInUseExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessageCorrectly()
    {
        // Arrange
        const string email = "test@example.com";
        const string expectedMessage = $"The email address: {email} is already in use by another existing account";

        // Act
        var exception = new EmailAlreadyInUseException(email);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(expectedMessage);
    }
}