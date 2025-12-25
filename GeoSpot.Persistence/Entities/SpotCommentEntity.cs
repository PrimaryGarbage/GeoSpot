namespace GeoSpot.Persistence.Entities;

internal class SpotCommentEntity : BaseAuditEntity
{
    public const string TableName = "spot_comments";
    
    public Guid SpotCommentId { get; set; }
    
    public Guid SpotId { get; set; }
    
    public Guid CreatorId { get; set; }
    
    public required string Text { get; set; }
    
    public SpotEntity? Spot { get; set; }
    public UserEntity? Creator { get; set; }
}