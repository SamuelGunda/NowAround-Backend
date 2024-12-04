using System.Text;

namespace NowAround.Api.Apis.Auth0.Utilities;

public static class PasswordUtils
{
    private static readonly Random Random = new();
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Numbers = "0123456789";
    private const string SpecialChars = "!@#$%^&*";

    public static string Generate(int length = 10)
    {
        if (length < 10)
            throw new ArgumentException("Password length must be at least 10 characters.");

        const string allChars = LowerCase + UpperCase + Numbers + SpecialChars;
        var password = new StringBuilder();

        password.Append(LowerCase[Random.Next(LowerCase.Length)]);
        password.Append(UpperCase[Random.Next(UpperCase.Length)]);
        password.Append(Numbers[Random.Next(Numbers.Length)]);
        password.Append(SpecialChars[Random.Next(SpecialChars.Length)]);

        for (var i = 4; i < length; i++)
        {
            password.Append(allChars[Random.Next(allChars.Length)]);
        }

        return new string(password.ToString().OrderBy(_ => Random.Next()).ToArray());
    }
}