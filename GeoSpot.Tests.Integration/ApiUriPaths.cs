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
        public static string Spot(Guid spotId) => $"/api/spots/{spotId}";
        public const string Spots = "/api/spots";
        public static string SpotView(Guid spotId) => $"/api/spots/{spotId}/view";
        public static string SpotReaction(Guid spotId) => $"/api/spots/{spotId}/reaction";
        public static string SpotComments(Guid spotId) => $"/api/spots/{spotId}/comments";
        public static string SpotComment(Guid commentId) => $"/api/spots/comments/{commentId}";
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