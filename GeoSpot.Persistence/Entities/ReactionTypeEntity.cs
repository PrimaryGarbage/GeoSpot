namespace GeoSpot.Persistence.Entities;

[ExcludeFromCodeCoverage]
internal class ReactionTypeEntity
{
    public const string TableName = "reaction_types";
    
    public Guid ReactionTypeId { get; set; }
    
    public required string Name { get; set; }
    
    public required string Emoji { get; set; }
}