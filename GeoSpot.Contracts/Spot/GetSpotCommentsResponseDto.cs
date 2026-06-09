namespace GeoSpot.Contracts.Spot;

public record GetSpotCommentsResponseDto(IEnumerable<SpotCommentDto> Comments);