using GeoSpot.Persistence.Repositories.Models.RefreshToken;
using GeoSpot.Persistence.Repositories.Models.User;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenModel?> GetRefreshTokenAsync(string tokenHash, CancellationToken ct = default);
    
    Task<(RefreshTokenModel?, UserModel?)> GetRefreshTokenWithUserAsync(string tokenHash, CancellationToken ct = default);
    
    Task<RefreshTokenModel> CreateRefreshTokenAsync(CreateRefreshTokenModel createModel, CancellationToken ct = default);
    
    Task DeleteRefreshTokenAsync(string tokenHash, CancellationToken ct = default);
    
    Task RevokeRefreshTokenAsync(string tokenHash, CancellationToken ct = default);
    
    Task DeleteAllUserRefreshTokensAsync(Guid userId, CancellationToken ct = default);
}