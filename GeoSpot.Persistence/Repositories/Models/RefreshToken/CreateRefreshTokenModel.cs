namespace GeoSpot.Persistence.Repositories.Models.RefreshToken;

[ExcludeFromCodeCoverage]
public class CreateRefreshTokenModel
{
    public required string TokenHash { get; set; }
    
    public Guid UserId { get; set; }
    
    public DateTime ExpiresAt { get; set; }
}