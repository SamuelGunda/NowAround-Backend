namespace NowAround.Api.Authentication.Exceptions;

public class EstablishmentNotFound : Exception
{
    public EstablishmentNotFound(string name) 
        : base($"The establishment with name: {name} was not found")
    {
    }
}