using GeoSpot.Common.Enums;

namespace GeoSpot.Contracts.Device;

public class RegisterDeviceRequestDto
{
    public required string Token { get; init; }
    
    public Platform Platform { get; init; }
}