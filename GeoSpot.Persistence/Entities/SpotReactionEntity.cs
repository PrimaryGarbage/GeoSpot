namespace GeoSpot.Persistence.Entities;

internal class SpotReactionEntity : BaseAuditEntity
{
    public const string TableName = "spot_reactions";
    
    public Guid SpotId { get; set; }
    
    public Guid CreatorId { get; set; }
    
    public Guid ReactionTypeId { get; set; }
    
    public SpotEntity? Spot { get; set; }
    public UserEntity? Creator { get; set; }
    public ReactionTypeEntity? ReactionType { get; set; }
}