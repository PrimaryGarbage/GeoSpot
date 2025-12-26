namespace GeoSpot.Persistence.Repositories.Models.RefreshToken;

[ExcludeFromCodeCoverage]
public class RefreshTokenModel
{
    public Guid RefreshTokenId { get; set; }
    
    public required string TokenHash { get; set; }
    
    public Guid UserId { get; set; }
    
    public bool Revoked { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime ExpiresAt { get; set; }
}