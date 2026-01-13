using GeoSpot.Contracts.Category;

namespace GeoSpot.Contracts.User;

public class UpdateCurrentUserCategoriesRequestDto
{
    public required IEnumerable<CategoryDto> Categories { get; set; }
}