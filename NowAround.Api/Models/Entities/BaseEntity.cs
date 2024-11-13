namespace NowAround.Api.Models.Entities;

public interface IBaseEntity
{
    public int Id { get; set; }
}

public abstract class BaseEntity : IBaseEntity
{
    public int Id { get; set; }
}