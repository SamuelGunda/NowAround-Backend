using NowAround.Application.Common.Helpers;

namespace NowAround.Application.UnitTests.Common.Helpers;

public class PasswordUtilsTests
{
    [Fact]
    public void Generate_ShouldThrowException_WhenLengthIsLessThan10()
    {
        // Arrange & Act
        var exception = Assert.Throws<ArgumentException>(() => PasswordUtils.Generate(9));

        // Assert
        Assert.Equal("Password length must be at least 10 characters.", exception.Message);
    }

    [Fact]
    public void Generate_ShouldReturnPasswordOfSpecifiedLength()
    {
        // Arrange
        const int length = 15;

        // Act
        var password = PasswordUtils.Generate(length);

        // Assert
        Assert.Equal(length, password.Length);
    }

    [Fact]
    public void Generate_ShouldContainAtLeastOneLowercaseLetter()
    {
        // Act
        var password = PasswordUtils.Generate();

        // Assert
        Assert.Matches("[a-z]", password);
    }

    [Fact]
    public void Generate_ShouldContainAtLeastOneUppercaseLetter()
    {
        // Act
        var password = PasswordUtils.Generate();

        // Assert
        Assert.Matches("[A-Z]", password);
    }

    [Fact]
    public void Generate_ShouldContainAtLeastOneNumber()
    {
        // Act
        var password = PasswordUtils.Generate();

        // Assert
        Assert.Matches("[0-9]", password);
    }

    [Fact]
    public void Generate_ShouldContainAtLeastOneSpecialCharacter()
    {
        // Act
        var password = PasswordUtils.Generate();

        // Assert
        Assert.Matches("[!@#$%^&*]", password);
    }

    [Fact]
    public void Generate_ShouldReturnRandomizedPasswords()
    {
        // Act
        var password1 = PasswordUtils.Generate();
        var password2 = PasswordUtils.Generate();

        // Assert
        Assert.NotEqual(password1, password2);
    }

    [Fact]
    public void Generate_ShouldReturnPasswordWithAllRequiredCharacterTypes()
    {
        // Act
        var password = PasswordUtils.Generate();

        // Assert
        Assert.Matches("[a-z]", password);
        Assert.Matches("[A-Z]", password);
        Assert.Matches("[0-9]", password);
        Assert.Matches("[!@#$%^&*]", password);
    }
}