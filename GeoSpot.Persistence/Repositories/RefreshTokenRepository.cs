using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Mappers;
using GeoSpot.Persistence.Repositories.Models.RefreshToken;
using GeoSpot.Persistence.Repositories.Models.User;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Persistence.Repositories;

[ExcludeFromCodeCoverage]
internal class RefreshTokenRepository : BaseGeoSpotRepository, IRefreshTokenRepository
{
    public RefreshTokenRepository(GeoSpotDbContext dbContext) : base(dbContext)
    {}
    
    public async Task<RefreshTokenModel?> GetRefreshTokenAsync(string tokenHash, CancellationToken ct = default)
    {
        RefreshTokenEntity? entity = await DbContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        
        return entity.MapToModelOrNull();
    }

    public async Task<(RefreshTokenModel?, UserModel?)> GetRefreshTokenWithUserAsync(string tokenHash, CancellationToken ct = default)
    {
        RefreshTokenEntity? entity = await DbContext.RefreshTokens.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        
        return (entity.MapToModelOrNull(), entity?.User.MapToModelOrNull());
    }

    public async Task<RefreshTokenModel> CreateRefreshTokenAsync(CreateRefreshTokenModel createModel, CancellationToken ct = default)
    {
        RefreshTokenEntity entity = createModel.MapToEntity();
        DbContext.RefreshTokens.Add(entity);
        
        await DbContext.SaveChangesAsync(ct);
        
        return entity.MapToModel();
    }

    public async Task DeleteRefreshTokenAsync(string tokenHash, CancellationToken ct = default)
    {
        RefreshTokenEntity? entity = await DbContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        if (entity is null) return;
        
        DbContext.Entry(entity).State = EntityState.Deleted;
        
        await DbContext.SaveChangesAsync(ct);
    }

    public async Task RevokeRefreshTokenAsync(string tokenHash, CancellationToken ct = default)
    {
        RefreshTokenEntity entity = await DbContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct)
            ?? throw new NotFoundException("Failed to find refresh token with the given token hash");
        
        entity.Revoked = true;
        
        await DbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAllUserRefreshTokensAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await DbContext.RefreshTokens.Where(x => x.UserId == userId).ToListAsync(ct);
        if (tokens.Count == 0) return;
        
        foreach(RefreshTokenEntity token in tokens)
            DbContext.Entry(token).State = EntityState.Deleted;
        
        await DbContext.SaveChangesAsync(ct);
    }
}