namespace NowAround.Api.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entity,string property, string message) 
        : base($"The {entity} with {property}: {message} was not found")
    {
    }
}