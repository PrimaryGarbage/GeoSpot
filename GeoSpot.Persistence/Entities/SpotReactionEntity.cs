namespace GeoSpot.Persistence.Entities;

public class SpotReactionEntity : BaseAuditEntity
{
    public Guid SpotId { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid ReactionTypeId { get; set; }
    
    public SpotEntity? Spot { get; set; }
    public UserEntity? User { get; set; }
    public ReactionTypeEntity? ReactionType { get; set; }
}