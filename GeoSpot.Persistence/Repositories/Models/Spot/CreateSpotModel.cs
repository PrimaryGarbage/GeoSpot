using GeoSpot.Common.Enums;

namespace GeoSpot.Persistence.Repositories.Models.Spot;

[ExcludeFromCodeCoverage]
public class CreateSpotModel
{
    public Guid CreatorId { get; set; }
    
    public Guid? BusinessProfileId { get; set; }
    
    public required string Title { get; set; }
    
    public string? Description { get; set; }
    
    public SpotType SpotType { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public int Radius { get; set; }
    
    public string? Address { get; set; }
    
    public DateTime StartsAt { get; set; }
    
    public DateTime EndsAt { get; set; }
}