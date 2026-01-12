namespace GeoSpot.Contracts.Auth;

[ExcludeFromCodeCoverage]
public class AccessTokenDto
{
    public required string AccessToken { get; init; }
    
    public required int AccessTokenExpiresInMinutes { get; init; }
    
    public required string RefreshToken { get; init; }
    
    public required int RefreshTokenExpiresInMinutes { get; init; }
}