using GeoSpot.Persistence.Repositories.Models.User;

namespace GeoSpot.Application.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(UserModel user);
    
    string GenerateRefreshToken();
    
    string HashToken(string token);
}
