using GeoSpot.Persistence.Repositories.Models.RefreshToken;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenModel> GetRefreshTokenAsync(Guid userId, string tokenHash, CancellationToken ct);
    
    Task<RefreshTokenModel> CreateRefreshTokenAsync(CreateRefreshTokenModel createModel, CancellationToken ct);
    
    Task<RefreshTokenModel> RotateRefreshTokenAsync(string oldTokenHash, CreateRefreshTokenModel createModel, CancellationToken ct);
    
    Task DeleteRefreshTokenAsync(string tokenHash, CancellationToken ct);
    
    Task RevokeRefreshTokenAsync(string tokenHash, CancellationToken ct);
}