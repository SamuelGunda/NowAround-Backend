using System.ComponentModel.DataAnnotations;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class User : BaseAccountEntity
{
    public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();
}