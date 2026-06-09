using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;

namespace GeoSpot.Application.Mappers;

internal static class SpotMapper
{
    public static SpotCommentDto MapToDto(this SpotCommentEntity input)
    {
        return new SpotCommentDto
        {
            CommentId = input.SpotCommentId,
            Text = input.Text,
        };
    }

    public static SpotDto MapToDto(this SpotEntity input)
    {
        return new SpotDto
        {
            SpotId = input.SpotId,
            BusinessProfileId = input.BusinessProfileId,
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
        };
    }

    public static SpotDto MapToDto(this SpotEntity input, int viewsCount)
    {
        return new SpotDto
        {
            SpotId = input.SpotId,
            BusinessProfileId = input.BusinessProfileId,
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
            ViewsCount = viewsCount
        };
    }

    public static SpotEntity MapToEntity(this CreateSpotRequestDto input)
    {
        return new SpotEntity
        {
            Title = input.Title,
            Description = input.Description,
            BusinessProfileId = input.BusinessProfileId,
            SpotType = input.SpotType,
            ImageUrl = input.ImageUrl,
            Latitude = input.Latitude,
            Longitude = input.Longitude,
            Radius = input.Radius,
            Address = input.Address,
            StartsAt = input.StartsAt,
            EndsAt = input.EndsAt
        };
    }

    public static void MapOntoEntity(this UpdateSpotRequestDto input, SpotEntity entity)
    {
        entity.Title = input.Title;
        entity.Description = input.Description;
        entity.SpotType = input.SpotType;
        entity.ImageUrl = input.ImageUrl;
        entity.Latitude = input.Latitude;
        entity.Longitude = input.Longitude;
        entity.Radius = input.Radius;
        entity.Address = input.Address;
    }
}