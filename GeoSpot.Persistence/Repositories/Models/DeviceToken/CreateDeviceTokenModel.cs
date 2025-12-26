using GeoSpot.Common.Enums;

namespace GeoSpot.Persistence.Repositories.Models.DeviceToken;

public class CreateDeviceTokenModel
{
    public Guid UserId { get; set; }
    
    public required string Token { get; set; }
    
    public Platform Platform { get; set; }
    
    public bool IsActive { get; set; }
}