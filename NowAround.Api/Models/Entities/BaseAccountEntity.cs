using System.ComponentModel.DataAnnotations;

namespace NowAround.Api.Models.Entities;

public interface IBaseAccountEntity
{
    public string Auth0Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public abstract class BaseAccountEntity : BaseEntity, IBaseAccountEntity
{
    [MaxLength(48)]
    public required string Auth0Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}