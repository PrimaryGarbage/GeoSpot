namespace GeoSpot.Persistence.Entities.Factories;

public static class UserEntityFactory
{
    public static UserEntity FromPhoneNumber(string phoneNumber)
    {
        return new UserEntity
        {
            PhoneNumber = phoneNumber,
            DisplayName = "Not Specified"
        };
    }
}