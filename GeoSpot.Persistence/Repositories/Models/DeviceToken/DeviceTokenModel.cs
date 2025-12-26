using GeoSpot.Common.Enums;

namespace GeoSpot.Persistence.Repositories.Models.DeviceToken;

[ExcludeFromCodeCoverage]
public class DeviceTokenModel
{
    public Guid DeviceTokenId { get; set; }
    
    public Guid UserId { get; set; }
    
    public required string Token { get; set; }
    
    public Platform Platform { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}