namespace GeoSpot.Persistence.Entities;

public class ReactionTypeEntity
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Emoji { get; set; }
}