namespace GeoSpot.Common;

[ExcludeFromCodeCoverage]
public static class CacheKeys
{
    public static string VerificationCodeModel(string phoneNumber) => $"verification_code_model_{phoneNumber}";
}