using GeoSpot.Persistence.Repositories.Models.RefreshToken;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenModel> GetRefreshTokenAsync(Guid userId, string tokenHash, CancellationToken ct = default);
    
    Task<RefreshTokenModel> CreateRefreshTokenAsync(CreateRefreshTokenModel createModel, CancellationToken ct = default);
    
    Task<RefreshTokenModel> RotateRefreshTokenAsync(string oldTokenHash, CreateRefreshTokenModel createModel, CancellationToken ct = default);
    
    Task DeleteRefreshTokenAsync(string tokenHash, CancellationToken ct = default);
    
    Task RevokeRefreshTokenAsync(string tokenHash, CancellationToken ct = default);
    
    Task DeleteAllUserRefreshTokensAsync(Guid userId, CancellationToken ct = default);
}