namespace NowAround.Api.Exceptions;

public class InvalidLocationException : Exception
{
    public InvalidLocationException(string message) : base(message)
    {
    }
}