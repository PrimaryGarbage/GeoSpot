using GeoSpot.Contracts.User.v1;
using GeoSpot.Persistence.Repositories.Models.User;

namespace GeoSpot.Application.Services.Mappers.User;

[ExcludeFromCodeCoverage]
internal static class UserMapper
{
    public static UserDto MapToDto(this UserModel input)
    {
        return new UserDto
        {
            UserId = input.UserId,
            AccountType = input.AccountType,
            AvatarUrl = input.AvatarUrl,
            BirthYear = input.BirthYear,
            DetectionRadius = input.DetectionRadius,
            DisplayName = input.DisplayName,
            Email = input.Email,
            Gender = input.Gender,
            PhoneNumber = input.PhoneNumber,
            IsPremium = input.IsPremium,
            IsVerified = input.IsVerified,
            PasswordHash = input.PasswordHash,
            LastLatitude = input.LastLatitude,
            LastLongitude = input.LastLongitude,
            LocationUpdatedAt = input.LocationUpdatedAt
        };
    }

    public static UserModel MapToModel(this UserDto input)
    {
        return new UserModel
        {
            UserId = input.UserId,
            AccountType = input.AccountType,
            AvatarUrl = input.AvatarUrl,
            BirthYear = input.BirthYear,
            DetectionRadius = input.DetectionRadius,
            DisplayName = input.DisplayName,
            Email = input.Email,
            Gender = input.Gender,
            PhoneNumber = input.PhoneNumber,
            IsPremium = input.IsPremium,
            IsVerified = input.IsVerified,
            PasswordHash = input.PasswordHash,
            LastLatitude = input.LastLatitude,
            LastLongitude = input.LastLongitude,
            LocationUpdatedAt = input.LocationUpdatedAt
        };
    }
}