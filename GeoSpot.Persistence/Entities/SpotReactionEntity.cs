namespace GeoSpot.Persistence.Entities;

public class SpotReactionEntity : IAuditEntity
{
    public const string TableName = "spot_reactions";
    
    public Guid SpotId { get; set; }
    
    public Guid CreatorId { get; set; }
    
    public Guid ReactionTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    
    public SpotEntity? Spot { get; set; }
    public UserEntity? Creator { get; set; }
    public ReactionTypeEntity? ReactionType { get; set; }
}