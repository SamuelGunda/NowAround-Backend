namespace NowAround.Application.Common.Exceptions;

public class EntityAlreadyExistsException : Exception
{
    public EntityAlreadyExistsException(string entity, string property, string value)
        : base($"The {entity} with {property}: {value} already exists")
    {
    }
}