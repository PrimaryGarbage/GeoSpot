namespace GeoSpot.Contracts.ReactionType;

public class GetReactionTypesResponseDto
{
    public required IEnumerable<ReactionTypeDto> ReactionTypes { get; set; }
}