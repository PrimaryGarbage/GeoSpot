namespace GeoSpot.Tests.Integration.Constants;

[ExcludeFromCodeCoverage]
internal static class UriConstants
{
    public static class Auth
    {
        public const string SendVerificationCode = "/api/auth/send-code";
        public const string VerifyVerificationCode = "/api/auth/verify-code";
        public const string RefreshAccessToken = "/api/auth/refresh";
        public const string LogoutUser = "/api/auth/logout";
    }
}