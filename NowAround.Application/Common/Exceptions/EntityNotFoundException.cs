namespace NowAround.Application.Common.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entity,string property, string value)
        : base($"The {entity} with {property}: {value} was not found")
    {
    }
}