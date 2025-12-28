using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Mappers;
using GeoSpot.Persistence.Repositories.Models.RefreshToken;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Persistence.Repositories;

[ExcludeFromCodeCoverage]
internal class RefreshTokenRepository : BaseGeoSpotRepository, IRefreshTokenRepository
{
    public RefreshTokenRepository(GeoSpotDbContext dbContext) : base(dbContext)
    {}
    
    public Task<RefreshTokenModel> GetRefreshTokenAsync(Guid userId, string tokenHash, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<RefreshTokenModel> CreateRefreshTokenAsync(CreateRefreshTokenModel createModel, CancellationToken ct = default)
    {
        RefreshTokenEntity entity = createModel.MapToEntity();
        DbContext.RefreshTokens.Add(entity);
        
        await DbContext.SaveChangesAsync(ct);
        
        return entity.MapToModel();
    }

    public Task<RefreshTokenModel> RotateRefreshTokenAsync(string oldTokenHash, CreateRefreshTokenModel createModel, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRefreshTokenAsync(string tokenHash, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task RevokeRefreshTokenAsync(string tokenHash, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAllUserRefreshTokensAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await DbContext.RefreshTokens.Where(x => x.UserId == userId).ToListAsync(ct);
        if(tokens.Count == 0) return;
        
        foreach(RefreshTokenEntity token in tokens)
            DbContext.Entry(token).State = EntityState.Deleted;
        
        await DbContext.SaveChangesAsync(ct);
    }
}