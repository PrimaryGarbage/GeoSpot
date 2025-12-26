namespace GeoSpot.Persistence.Repositories.Models.SpotComment;

[ExcludeFromCodeCoverage]
public class CreateSpotCommentModel
{
    public Guid SpotId { get; set; }
    
    public Guid CreatorId { get; set; }
    
    public required string Text { get; set; }
}