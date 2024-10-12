namespace NowAround.Api.Authentication.Exceptions;

public class EstablishmentAlreadyExistsException : Exception
{
    public EstablishmentAlreadyExistsException(string name) 
        : base($"The establishment with name: {name} already exists")
    {
    }
}