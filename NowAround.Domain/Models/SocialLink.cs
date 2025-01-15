using System.ComponentModel.DataAnnotations;
using NowAround.Domain.Common;

namespace NowAround.Domain.Models;

public class SocialLink : BaseEntity
{
    [MaxLength(32)]
    public required string Name { get; set; }
    [MaxLength(256)]
    public required string Url { get; set; }
    public int EstablishmentId { get; set; }
    public Establishment Establishment { get; set; }
}