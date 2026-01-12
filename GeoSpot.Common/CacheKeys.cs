namespace GeoSpot.Common;

public static class CacheKeys
{
    public static string VerificationCodeEntity(string phoneNumber) => $"verification_code_entity_{phoneNumber}";
}