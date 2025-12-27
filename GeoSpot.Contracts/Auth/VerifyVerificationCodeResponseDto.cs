using System.Diagnostics.CodeAnalysis;

namespace GeoSpot.Contracts.Auth;

[ExcludeFromCodeCoverage]
public class VerifyVerificationCodeResponseDto
{
    public required string AccessToken { get; set; }
    
    public required int AccessTokenExpiresInMinutes { get; set; }
    
    public required string RefreshToken { get; set; }
    
    public required int RefreshTokenExpiresInMinutes { get; set; }
}