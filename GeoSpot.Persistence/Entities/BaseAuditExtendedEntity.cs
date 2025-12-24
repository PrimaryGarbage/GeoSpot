namespace GeoSpot.Persistence.Entities;

public class BaseAuditExtendedEntity : BaseAuditEntity
{
    public DateTime UpdatedAt { get; set; }
}