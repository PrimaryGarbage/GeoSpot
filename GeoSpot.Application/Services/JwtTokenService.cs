using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Common.ConfigurationSections;
using GeoSpot.Persistence.Repositories.Models.User;
using GeoSpot.Common.Constants;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GeoSpot.Application.Services;

[ExcludeFromCodeCoverage]
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtConfigurationSection _jwtConfiguration;

    public JwtTokenService(IOptions<JwtConfigurationSection> jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration.Value;
    }
    
    public string GenerateAccessToken(UserModel userModel)
    {
        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, userModel.UserId.ToString()),
            new Claim(ClaimTypes.MobilePhone, userModel.PhoneNumber),
            new Claim(ClaimTypes.Role, AuthorizationConstants.UserRoleName)
        ];
        
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
        
        JwtSecurityToken token = new(issuer: _jwtConfiguration.Issuer, audience: _jwtConfiguration.Audience,
            claims: claims, expires: DateTime.UtcNow.AddMicroseconds(_jwtConfiguration.AccessTokenLifespanMinutes),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        byte[] randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
    
    public string HashToken(string token)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(token);
        return Convert.ToBase64String(SHA256.HashData(bytes));
    }
}