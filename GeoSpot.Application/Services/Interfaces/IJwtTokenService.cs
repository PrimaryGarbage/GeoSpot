using GeoSpot.Persistence.Entities;

namespace GeoSpot.Application.Services.Interfaces;

public interface IJwtTokenService
{
    public int AccessTokenLifespanMinutes { get; }

    public int RefreshTokenLifespanMinutes { get; }
    
    string GenerateAccessToken(UserEntity user);
    
    string GenerateRefreshToken();
    
    string HashToken(string token);
}
