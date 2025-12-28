using System.Diagnostics.CodeAnalysis;

namespace GeoSpot.Tests.Integration.ApiTests;

[ExcludeFromCodeCoverage]
internal static class GeoSpotUriConstants
{
    public static class AuthUri
    {
        public const string SendVerificationCode = "/api/auth/send-code";
        public const string VerifyVerificationCode = "/api/auth/verify-code";
    }
}