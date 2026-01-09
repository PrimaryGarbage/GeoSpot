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
    public RefreshTokenRepository(GeoSpotDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {}
    
    public async Task<RefreshTokenModel?> GetRefreshTokenAsync(string tokenHash, CancellationToken ct = default)
    {
        RefreshTokenEntity? entity = await DbContext.RefreshTokens.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        
        return entity.MapToModelOrNull();
    }

    public async Task<(RefreshTokenModel?, UserModel?)> GetRefreshTokenWithUserAsync(string tokenHash, CancellationToken ct = default)
    {
        RefreshTokenEntity? entity = await DbContext.RefreshTokens
            .AsNoTracking()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        
        return (entity.MapToModelOrNull(), entity?.User.MapToModelOrNull());
    }

    public async Task<RefreshTokenModel> CreateRefreshTokenAsync(CreateRefreshTokenModel createModel, CancellationToken ct = default)
    {
        RefreshTokenEntity entity = createModel.MapToEntity();
        DbContext.RefreshTokens.Add(entity);
        
        await SaveChangesAsync(ct);
        
        return entity.MapToModel();
    }

    public async Task DeleteRefreshTokenAsync(string tokenHash, CancellationToken ct = default)
    {
        RefreshTokenEntity? entity = await DbContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        if (entity is null) return;
        
        DbContext.Entry(entity).State = EntityState.Deleted;
        
        await SaveChangesAsync(ct);
    }

    public async Task RevokeRefreshTokenAsync(string tokenHash, CancellationToken ct = default)
    {
        RefreshTokenEntity entity = await DbContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct)
            ?? throw new NotFoundException("Failed to find refresh token with the given token hash");
        
        entity.Revoked = true;
        
        await SaveChangesAsync(ct);
    }

    public async Task DeleteAllUserRefreshTokensAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await DbContext.RefreshTokens.Where(x => x.UserId == userId).ToListAsync(ct);
        if (tokens.Count == 0) return;
        
        DbContext.RemoveRange(tokens);
        
        await SaveChangesAsync(ct);
    }
}