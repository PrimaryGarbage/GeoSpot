namespace GeoSpot.Tests.Integration.ApiTests.Constants;

[ExcludeFromCodeCoverage]
internal static class GeoSpotUriConstants
{
    public static class AuthUri
    {
        public const string SendVerificationCode = "/api/auth/send-code";
        public const string VerifyVerificationCode = "/api/auth/verify-code";
    }
}