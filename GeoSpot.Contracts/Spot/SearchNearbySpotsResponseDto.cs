namespace GeoSpot.Contracts.Spot;

public class SearchNearbySpotsResponseDto
{
    public required IEnumerable<SpotDto> Spots { get; set; }
}