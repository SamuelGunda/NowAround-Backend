using NowAround.Api.Models.Entities;

namespace NowAround.Api.Models.Domain;

public class RatingStatistic : BaseEntity
{
    public int OneStar { get; set; } = 0;
    public int TwoStars { get; set; } = 0;
    public int ThreeStars { get; set; } = 0;
    public int FourStars { get; set; } = 0;
    public int FiveStars { get; set; } = 0;
    
    public int EstablishmentId { get; set; }
    public Establishment Establishment { get; set; }
    
    
    public  ICollection<Review> Reviews { get; set; } = new List<Review>();
}