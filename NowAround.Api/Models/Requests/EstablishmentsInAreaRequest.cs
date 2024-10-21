using Newtonsoft.Json;

namespace NowAround.Api.Models.Requests;

public class EstablishmentsInAreaRequest
{
    [JsonProperty("northWestCorner")]
    public List<double> NWCorner { get; set; } = [];
    [JsonProperty("southEastCorner")]
    public List<double> SECorner { get; set; } = [];
    
    public void ValidateProperties()
    {
        if (NWCorner.Count != 2 || SECorner.Count != 2)
        {
            throw new ArgumentNullException(nameof(NWCorner));
        }
        if (NWCorner[0] >= SECorner[0] || NWCorner[1] <= SECorner[1])
        {
            throw new ArgumentException("Invalid coordinates");
        }
        if (NWCorner[0] < -90 || NWCorner[0] > 90 || NWCorner[1] < -180 || NWCorner[1] > 180)
        {
            throw new ArgumentException("Invalid coordinates");
        }
    }
}