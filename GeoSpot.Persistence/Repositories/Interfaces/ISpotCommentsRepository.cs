using GeoSpot.Persistence.Repositories.Models.SpotComment;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface ISpotCommentsRepository
{
    Task<IEnumerable<SpotCommentModel>> GetSpotCommentsAsync(Guid spotId, CancellationToken ct = default);
    
    Task<SpotCommentModel> CreateSpotCommentAsync(CreateSpotCommentModel createModel, CancellationToken ct = default);
    
    Task DeleteSpotCommentAsync(Guid spotCommentId, CancellationToken ct = default);
}