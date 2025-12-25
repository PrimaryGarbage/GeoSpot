namespace GeoSpot.Persistence.Entities;

internal enum Platform { Ios, Android }

internal class DeviceTokenEntity : BaseAuditEntity
{
    public const string TableName = "device_tokens";
    
    public Guid DeviceTokenId { get; set; }
    
    public Guid UserId { get; set; }
    
    public required string Token { get; set; }
    
    public Platform Platform { get; set; }
    
    public bool IsActive { get; set; }
    
    public UserEntity? User { get; set; }
}