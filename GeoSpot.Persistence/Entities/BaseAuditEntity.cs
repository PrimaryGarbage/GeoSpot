namespace GeoSpot.Persistence.Entities;

internal class BaseAuditEntity
{
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
}