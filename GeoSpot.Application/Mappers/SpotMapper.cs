using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;

namespace GeoSpot.Application.Mappers;

internal static class SpotMapper
{
    public static SpotDto MapToDto(this SpotEntity input)
    {
        return new SpotDto
        {
            SpotId = input.SpotId,
            Title = input.Title,
            Description = input.Description,
            SpotType = input.SpotType,
            ImageUrl = input.ImageUrl,
            Latitude = input.Latitude,
            Longitude = input.Longitude,
            Radius = input.Radius,
            Address = input.Address,
            StartsAt = input.StartsAt,
            EndsAt = input.EndsAt,
            ViewsCount = input.ViewsCount
        };
    }
}