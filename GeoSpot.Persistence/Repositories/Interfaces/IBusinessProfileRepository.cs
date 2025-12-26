using GeoSpot.Persistence.Repositories.Models.BusinessProfile;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface IBusinessProfileRepository
{
    Task<BusinessProfileModel> CreateBusinessProfileAsync(CreateBusinessProfileModel createModel, CancellationToken ct);
    
    Task<BusinessProfileModel> GetBusinessProfileAsync(Guid businessProfileId, CancellationToken ct);
    
    Task UpdateBusinessProfileId(UpdateBusinessProfileModel updateModel, CancellationToken ct);
}