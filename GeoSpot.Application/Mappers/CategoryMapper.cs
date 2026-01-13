using GeoSpot.Contracts.Category;
using GeoSpot.Persistence.Entities;

namespace GeoSpot.Application.Mappers;

internal static class CategoryMapper
{
    public static CategoryDto MapToDto(this CategoryEntity input)
    {
        return new CategoryDto
        {
            CategoryId = input.CategoryId,
            Name = input.Name,
            IconData = input.IconData,
            Color = input.Color,
            SortOrder = input.SortOrder
        };
    }
}