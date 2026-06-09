namespace GeoSpot.Contracts.Spot;

public class SpotCommentDto
{
    public Guid CommentId { get; init; }
    
    public required string Text { get; init; }
}