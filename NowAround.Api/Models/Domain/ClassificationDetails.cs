using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Models.Domain;

//TODO: May use in the future
public class ClassificationDetails : BaseEntity
{
    public required PriceCategory PriceCategory { get; set; }
    public virtual ICollection<EstablishmentCategory> EstablishmentCategories { get; set; } = new List<EstablishmentCategory>();
    public virtual ICollection<EstablishmentTag> EstablishmentTags { get; set; } = new List<EstablishmentTag>();
}