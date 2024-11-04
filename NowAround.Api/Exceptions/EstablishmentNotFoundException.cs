namespace NowAround.Api.Exceptions;

public sealed class EstablishmentNotFoundException : Exception
{
    public EstablishmentNotFoundException(string message) 
        : base($"The establishment with {message} was not found")
    {
    }
}