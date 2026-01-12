namespace GeoSpot.Tests.Integration.Constants;

internal static class UriConstants
{
    public static class Auth
    {
        public const string SendVerificationCode = "/api/auth/send-code";
        public const string VerifyVerificationCode = "/api/auth/verify-code";
        public const string RefreshAccessToken = "/api/auth/refresh";
        public const string LogoutUser = "/api/auth/logout";
    }

    public static class Users
    {
        public const string GetCurrentUser = "/api/users/me";
    }
}