namespace NowAround.Api.Apis.Auth0.Exceptions;

public class EstablishmentAlreadyExistsException : Exception
{
    public EstablishmentAlreadyExistsException(string name) 
        : base($"The establishment with name: {name} already exists")
    {
    }
}