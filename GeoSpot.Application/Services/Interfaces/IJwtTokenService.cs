using GeoSpot.Persistence.Repositories.Models.User;

namespace GeoSpot.Application.Services.Interfaces;

public interface IJwtTokenService
{
    public int AccessTokenLifespanMinutes { get; }

    public int RefreshTokenLifespanMinutes { get; }
    
    string GenerateAccessToken(UserModel user);
    
    string GenerateRefreshToken();
    
    string HashToken(string token);
}
