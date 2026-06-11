namespace GeoSpot.Contracts.Spot;

public record UpdateSpotCategoriesRequestDto(ICollection<Guid> CategoryIds);