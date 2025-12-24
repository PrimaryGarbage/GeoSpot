namespace GeoSpot.Persistence.Entities;

public class BusinessProfileEntity : BaseAuditExtendedEntity
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public string? LogoUrl { get; set; }
    
    public string? Address { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public int Radius { get; set; }
    
    public string? PhoneNumber  { get; set; }
    
    public string? Email { get; set; }
    
    public string? WebsiteUrl { get; set; }
    
    public bool IsVerified { get; set; }
    
    public int NotificationBalance { get; set; }
    
    
    public UserEntity? User { get; set; }
    public CategoryEntity? Category { get; set; }
}