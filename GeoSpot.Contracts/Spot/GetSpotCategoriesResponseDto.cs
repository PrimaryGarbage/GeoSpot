using GeoSpot.Contracts.Category;

namespace GeoSpot.Contracts.Spot;

public sealed record GetSpotCategoriesResponseDto(IEnumerable<CategoryDto> Categories);