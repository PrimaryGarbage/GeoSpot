using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Repositories.Models.RefreshToken;

namespace GeoSpot.Persistence.Repositories.Mappers;

[ExcludeFromCodeCoverage]
internal static class RefreshTokenMapper
{
    public static RefreshTokenEntity MapToEntity(this CreateRefreshTokenModel input)
    {
        return new RefreshTokenEntity
        {
            TokenHash = input.TokenHash,
            ExpiresAt = input.ExpiresAt,
            UserId = input.UserId
        };
    }
    
    public static RefreshTokenModel MapToModel(this RefreshTokenEntity input)
    {
        return new RefreshTokenModel
        {
            UserId = input.UserId,
            TokenHash = input.TokenHash,
            ExpiresAt = input.ExpiresAt,
            CreatedAt = input.CreatedAt,
            RefreshTokenId = input.RefreshTokenId,
            Revoked = input.Revoked
        };
    }

    public static RefreshTokenModel? MapToModelOrNull(this RefreshTokenEntity? input)
    {
        return input?.MapToModel();
    }
}