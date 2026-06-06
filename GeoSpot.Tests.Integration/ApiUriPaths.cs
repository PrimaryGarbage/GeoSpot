namespace GeoSpot.Tests.Integration;

internal static class ApiUriPaths
{
    public static class AuthUri
    {
        public const string SendVerificationCode = "/api/auth/send-code";
        public const string VerifyVerificationCode = "/api/auth/verify-code";
        public const string RefreshAccessToken = "/api/auth/refresh";
        public const string LogoutUser = "/api/auth/logout";
    }

    public static class UsersUri
    {
        public const string CurrentUser = "/api/users/me";
        public const string CurrentUserCategories = "/api/users/me/categories";
    }

    public static class SpotsUri
    {
        public const string SearchNearbySpots = "/api/spots/nearby";
        public static string SpotById(Guid id) => $"/api/spots/{id}";
        public const string Spots = "/api/spots";
        public static string SpotViewById(Guid id) => $"/api/spots/{id}/view";
        public static string SpotReactionById(Guid id) => $"/api/spots/{id}/reaction";
    }
    
    public static class ReactionTypesUri
    {
        public const string ReactionTypes = "/api/reaction-types";
    }
    
    public static class DevicesUri
    {
        public const string Register = "/api/devices/register";
        public static string Unregister(Guid deviceTokenId) => $"/api/devices/{deviceTokenId}";
    }
}