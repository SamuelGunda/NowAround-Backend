using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class User : BaseAccountEntity
{
    [MaxLength(32)]
    public required string FullName { get; set; }
    public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();
}