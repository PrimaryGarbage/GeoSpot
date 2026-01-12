using GeoSpot.Common.Enums;

namespace GeoSpot.Persistence.Entities;

public class UserEntity : IAuditEntity
{
    public const string TableName = "users";
    
    public Guid UserId { get; set; }

    public required string PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }
    
    public string? PasswordSalt { get; set; }
    
    public AccountType AccountType { get; set; }
    
    public bool IsVerified { get; set; }
    
    public int DetectionRadius { get; set; }
    
    public bool IsPremium { get; set; }
    
    public double LastLatitude { get; set; }
    
    public double LastLongitude { get; set; }
    
    public DateTime LocationUpdatedAt { get; set; }
    
    public required string DisplayName { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    public int BirthYear { get; set; }

    public Gender Gender { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public IEnumerable<CategoryEntity>? Categories { get; set; }
    public IEnumerable<UserSpotViewEntity>? UserSpotViews { get; set; }
    public IEnumerable<BusinessProfileEntity>? BusinessProfiles { get; set; }
    public IEnumerable<SpotEntity>? CreatedSpots { get; set; }
    public IEnumerable<SpotCommentEntity>? Comments { get; set; }
    public IEnumerable<SpotReactionEntity>? Reactions { get; set; }
    public IEnumerable<DeviceTokenEntity>? DeviceTokens { get; set; }
}