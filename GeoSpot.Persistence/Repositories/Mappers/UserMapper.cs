using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Repositories.Models.User;

namespace GeoSpot.Persistence.Repositories.Mappers;

[ExcludeFromCodeCoverage]
internal static class UserMapper
{
    public static UserModel MapToModel(this UserEntity input)
    {
        return new UserModel
        {
            UserId = input.UserId,
            AccountType = input.AccountType,
            AvatarUrl = input.AvatarUrl,
            BirthYear = input.BirthYear,
            DetectionRadius = input.DetectionRadius,
            LastLatitude = input.LastLatitude,
            LastLongitude = input.LastLongitude,
            DisplayName = input.DisplayName,
            Email = input.Email,
            Gender = input.Gender,
            PhoneNumber = input.PhoneNumber,
            IsPremium = input.IsPremium,
            IsVerified = input.IsVerified,
            LocationUpdatedAt = input.LocationUpdatedAt,
            PasswordHash = input.PasswordHash
        };
    }

    public static UserModel? MapToModelOrNull(this UserEntity? input)
    {
        return input?.MapToModel();
    }
}