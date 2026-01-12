namespace GeoSpot.Persistence.Entities;

public class RefreshTokenEntity : IAuditEntity
{
    public const string TableName = "refresh_tokens";
    
    public Guid RefreshTokenId { get; set; }
    
    public required string TokenHash { get; set; }
    
    public Guid UserId { get; set; }
    
    public bool Revoked { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public UserEntity? User { get; set; }
}