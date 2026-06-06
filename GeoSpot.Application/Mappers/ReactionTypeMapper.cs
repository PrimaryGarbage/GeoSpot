using GeoSpot.Contracts.ReactionType;
using GeoSpot.Persistence.Entities;

namespace GeoSpot.Application.Mappers;

internal static class ReactionTypeMapper
{
    public static ReactionTypeDto MapToDto(this ReactionTypeEntity input)
    {
        return new()
        {
            ReactionTypeId = input.ReactionTypeId,
            Name = input.Name,
            Emoji = input.Emoji
        };
    }
}