using NowAround.Application.Dtos;

namespace NowAround.Application.Requests;

public class EstablishmentRegisterRequest
{
    public EstablishmentInfo EstablishmentInfo { get; set; }
    
    public OwnerInfo OwnerInfo { get; set; }
    
    public void ValidateProperties()
    {
        if (EstablishmentInfo == null)
        {
            throw new ArgumentNullException(nameof(EstablishmentInfo));
        }
        
        if (OwnerInfo == null)
        {
            throw new ArgumentNullException(nameof(OwnerInfo));
        }
        
        EstablishmentInfo.ValidateProperties();
        OwnerInfo.ValidateProperties();
    }
}