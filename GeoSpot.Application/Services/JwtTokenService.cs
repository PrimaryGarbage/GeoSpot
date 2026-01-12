using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Common.ConfigurationSections;
using GeoSpot.Common.Constants;
using GeoSpot.Persistence.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GeoSpot.Application.Services;

internal sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtConfigurationSection _jwtConfiguration;

    public JwtTokenService(IOptions<JwtConfigurationSection> jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration.Value;
    }
    
    public int AccessTokenLifespanMinutes => _jwtConfiguration.AccessTokenLifespanMinutes;
    
    public int RefreshTokenLifespanMinutes => _jwtConfiguration.RefreshTokenLifespanMinutes;
    
    public string GenerateAccessToken(UserEntity userModel)
    {
        List<Claim> claims = [
            new Claim(ClaimsConstants.UserId, userModel.UserId.ToString()),
            new Claim(ClaimsConstants.PhoneNumber, userModel.PhoneNumber),
            new Claim(ClaimsConstants.Role, AuthorizationConstants.UserRoleName),
            new Claim(ClaimsConstants.Email, userModel.Email ?? string.Empty),
        ];
        
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
        
        JwtSecurityToken token = new(issuer: _jwtConfiguration.Issuer, audience: _jwtConfiguration.Audience,
            claims: claims, expires: DateTime.UtcNow.AddMinutes(_jwtConfiguration.AccessTokenLifespanMinutes),
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