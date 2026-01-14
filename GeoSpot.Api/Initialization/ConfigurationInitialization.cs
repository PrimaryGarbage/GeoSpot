using GeoSpot.Common.ConfigurationSections;

namespace GeoSpot.Api.Initialization;

[ExcludeFromDescription]
internal static class ConfigurationInitialization
{
    public static void BindConfigurationSections(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfigurationSection>(configuration.GetSection(JwtConfigurationSection.SectionName));
        services.Configure<VerificationCodeConfigurationSection>(configuration.GetSection(VerificationCodeConfigurationSection.SectionName));
        services.Configure<GeolocationConfigurationSection>(configuration.GetSection(GeolocationConfigurationSection.SectionName));
    }
}