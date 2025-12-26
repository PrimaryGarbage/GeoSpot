using GeoSpot.Persistence.Repositories.Models.Spot;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface ISpotRepository
{
    Task<IEnumerable<SpotModel>> GetNearbySpotsAsync(double latitude, double longitude, int radius, CancellationToken ct);
    
    Task<SpotModel> GetSpotAsync(Guid spotId, CancellationToken ct);
    
    Task<SpotModel> CreateSpotAsync(CreateSpotModel createModel, CancellationToken ct);
    
    Task UpdateSpotAsync(UpdateSpotModel updateModel, CancellationToken ct);
    
    Task DeleteSpotAsync(Guid spotId, CancellationToken ct);
    
    Task IncrementSpotViewCountAsync(Guid spotId, CancellationToken ct);
}