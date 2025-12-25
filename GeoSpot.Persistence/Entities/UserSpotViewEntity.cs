namespace GeoSpot.Persistence.Entities;

internal class UserSpotViewEntity : BaseAuditEntity
{
    public const string TableName = "user_spot_views";
    
    public Guid UserId { get; set; }
    
    public Guid SpotId { get; set; }
    
    public bool Viewed { get; set; }
    
    public DateTime ViewedAt { get; set; }
    
    public UserEntity? User { get; set; }
    public SpotEntity? Spot { get; set; }
}