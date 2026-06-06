using GeoSpot.Common.Enums;

namespace GeoSpot.Contracts.Device;

public class RegisterDeviceRequestDto
{
    public required string Token { get; set; }
    
    public Platform Platform { get; set; }
}