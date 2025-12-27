using GeoSpot.Persistence.Repositories.Models.DeviceToken;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface IDeviceTokenRepository
{
    Task<DeviceTokenModel> CreateDeviceTokenAsync(CreateDeviceTokenModel createModel, CancellationToken ct = default);
    
    Task DeleteDeviceTokenAsync(string token, CancellationToken ct = default);
}