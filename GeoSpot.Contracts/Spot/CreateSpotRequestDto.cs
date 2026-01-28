using GeoSpot.Common.Enums;

namespace GeoSpot.Contracts.Spot;

public class CreateSpotRequestDto
{
    public required string Title { get; init; }
    
    public string? Description { get; init; }
    
    public Guid? BusinessProfileId { get; init; }
    
    public SpotType SpotType { get; init; }
    
    public string? ImageUrl { get; init; }
    
    public double Latitude { get; init; }
    
    public double Longitude { get; init; }
    
    public int Radius { get; init; }
    
    public string? Address { get; init; }
    
    public DateTime StartsAt { get; init; }
    
    public DateTime EndsAt { get; init; }
}