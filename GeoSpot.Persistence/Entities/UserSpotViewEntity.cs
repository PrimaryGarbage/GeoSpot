namespace GeoSpot.Persistence.Entities;

public class UserSpotViewEntity : BaseAuditEntity
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid SpotId { get; set; }
    
    public bool Viewed { get; set; }
    
    public DateTime ViewedAt { get; set; }
    
    public UserEntity? User { get; set; }
    public SpotEntity? Spot { get; set; }
}