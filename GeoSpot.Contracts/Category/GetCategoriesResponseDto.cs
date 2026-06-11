namespace GeoSpot.Contracts.Category;

public record GetCategoriesResponseDto(IEnumerable<CategoryDto> Categories);