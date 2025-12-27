using GeoSpot.Common.Enums;

namespace GeoSpot.Persistence.Repositories.Models.User;

[ExcludeFromCodeCoverage]
public class CreateUserModel
{
    public static CreateUserModel FromPhoneNumber(string phoneNumber)
    {
        return new CreateUserModel
        {
            PhoneNumber = phoneNumber,
            AccountType = AccountType.User,
            DisplayName = phoneNumber,
            Gender = Gender.NotSpecified
        };
    }
    
    public required string PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }
    
    public string? PasswordSalt { get; set; }
    
    public AccountType AccountType { get; set; }
    
    public bool IsVerified { get; set; }
    
    public int DetectionRadius { get; set; }
    
    public bool IsPremium { get; set; }
    
    public required string DisplayName { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    public int BirthYear { get; set; }

    public Gender Gender { get; set; }
}