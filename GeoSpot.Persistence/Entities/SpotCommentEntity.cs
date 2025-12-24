namespace GeoSpot.Persistence.Entities;

public class SpotCommentEntity : BaseAuditEntity
{
    public Guid Id { get; set; }
    
    public Guid SpotId { get; set; }
    
    public Guid UserId { get; set; }
    
    public required string Text { get; set; }
    
    public SpotEntity? Spot { get; set; }
    public UserEntity? User { get; set; }
}