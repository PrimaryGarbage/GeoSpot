namespace GeoSpot.Persistence.Repositories.Models.SpotComment;

[ExcludeFromCodeCoverage]
public class SpotCommentModel
{
    public Guid SpotCommentId { get; set; }
    
    public Guid SpotId { get; set; }
    
    public Guid CreatorId { get; set; }
    
    public required string Text { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}