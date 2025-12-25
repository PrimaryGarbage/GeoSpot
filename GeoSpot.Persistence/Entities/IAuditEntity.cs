namespace GeoSpot.Persistence.Entities;

internal interface IAuditEntity
{
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}