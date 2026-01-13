using GeoSpot.Contracts.User;
using GeoSpot.Persistence.Entities;

namespace GeoSpot.Application.Mappers;

internal static class UserMapper
{
    public static UserDto MapToDto(this UserEntity input)
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
            LastLatitude = input.LastLatitude,
            LastLongitude = input.LastLongitude,
            LocationUpdatedAt = input.LocationUpdatedAt
        };
    }

    public static UserEntity MapToEntity(this UserDto input)
    {
        return new UserEntity
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

    public static void MapOntoEntity(this UpdateCurrentUserRequestDto input, UserEntity target)
    {
        target.Email = input.Email;
        target.DetectionRadius = input.DetectionRadius;
        target.DisplayName = input.DisplayName;
        target.AvatarUrl = input.AvatarUrl;
        target.BirthYear = input.BirthYear;
        target.Gender = input.Gender;
    }
}