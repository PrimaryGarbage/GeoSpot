using GeoSpot.Contracts.Category;

namespace GeoSpot.Contracts.User;

public class GetCurrentUserCategoriesResponseDto
{
    public required IEnumerable<CategoryDto> Categories { get; set; }
}