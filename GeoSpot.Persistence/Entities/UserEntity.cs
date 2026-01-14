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
    
    public required string DisplayName { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    public int BirthYear { get; set; }

    public Gender Gender { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<CategoryEntity>? Categories { get; set; }
    public ICollection<UserSpotViewEntity>? UserSpotViews { get; set; }
    public IEnumerable<BusinessProfileEntity>? BusinessProfiles { get; set; }
    public IEnumerable<SpotEntity>? CreatedSpots { get; set; }
    public ICollection<SpotCommentEntity>? Comments { get; set; }
    public ICollection<SpotReactionEntity>? Reactions { get; set; }
    public ICollection<DeviceTokenEntity>? DeviceTokens { get; set; }
}