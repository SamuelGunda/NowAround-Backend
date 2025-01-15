using NowAround.Domain.Common;

namespace NowAround.Domain.Models;

public class RatingStatistic : BaseEntity
{
    public int OneStar { get; set; }
    public int TwoStars { get; set; }
    public int ThreeStars { get; set; }
    public int FourStars { get; set; }
    public int FiveStars { get; set; }
    
    public int EstablishmentId { get; set; }
    public Establishment Establishment { get; set; }
    
    public  ICollection<Review> Reviews { get; set; } = new List<Review>();
}