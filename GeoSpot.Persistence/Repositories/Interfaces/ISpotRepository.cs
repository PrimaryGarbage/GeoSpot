using GeoSpot.Persistence.Repositories.Models.Spot;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface ISpotRepository
{
    Task<IEnumerable<SpotModel>> GetNearbySpotsAsync(double latitude, double longitude, int radius, CancellationToken ct = default);
    
    Task<SpotModel> GetSpotAsync(Guid spotId, CancellationToken ct = default);
    
    Task<SpotModel> CreateSpotAsync(CreateSpotModel createModel, CancellationToken ct = default);
    
    Task UpdateSpotAsync(UpdateSpotModel updateModel, CancellationToken ct = default);
    
    Task DeleteSpotAsync(Guid spotId, CancellationToken ct = default);
    
    Task IncrementSpotViewCountAsync(Guid spotId, CancellationToken ct = default);
}