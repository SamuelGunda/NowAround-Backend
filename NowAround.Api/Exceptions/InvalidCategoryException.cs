namespace NowAround.Api.Exceptions;

public sealed class InvalidCategoryException : Exception
{
    public InvalidCategoryException() : base("Invalid category")
    {
    }
    
    public InvalidCategoryException(string message) : base(message)
    {
    }
}