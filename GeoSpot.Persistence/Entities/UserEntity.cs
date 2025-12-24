namespace GeoSpot.Persistence.Entities;

public enum AccountType { User, Business };

public enum Gender { NotSpecified, Male, Female, Other};

public class UserEntity : BaseAuditExtendedEntity
{
    public Guid Id { get; set; }

    public required string PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }
    
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
    
    public ICollection<CategoryEntity>? Categories { get; set; }
}