namespace NowAround.Api.Models.Entities;

public interface IEntity
{
    public int Id { get; set; }
}

public abstract class BaseEntity : IEntity
{
    public int Id { get; set; }
}