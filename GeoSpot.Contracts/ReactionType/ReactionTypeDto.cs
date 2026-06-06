namespace GeoSpot.Contracts.ReactionType;

public class ReactionTypeDto
{
    public Guid ReactionTypeId { get; set; }
    
    public required string Name { get; set; }
    
    public required string Emoji { get; set; }
}