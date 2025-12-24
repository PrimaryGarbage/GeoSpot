namespace GeoSpot.Persistence.Entities;

public enum Platform { Ios, Android }

public class DeviceTokenEntity : BaseAuditExtendedEntity
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public required string Token { get; set; }
    
    public Platform Platform { get; set; }
    
    public bool IsActive { get; set; }
}