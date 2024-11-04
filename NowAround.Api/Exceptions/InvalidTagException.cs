namespace NowAround.Api.Exceptions;

public sealed class InvalidTagException : Exception
{
    public InvalidTagException() : base("Invalid tag")
    {
    }
    
    public InvalidTagException(string message) : base(message)
    {
    }
}