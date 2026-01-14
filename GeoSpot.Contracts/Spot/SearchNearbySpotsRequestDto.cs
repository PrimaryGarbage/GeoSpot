namespace GeoSpot.Contracts.Spot;

public class SearchNearbySpotsRequestDto
{
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public int Radius { get; set; }
}