using GeoSpot.Common.ConfigurationSections;
using IConfigurationSection = GeoSpot.Common.ConfigurationSections.IConfigurationSection;

namespace GeoSpot.Api.Initialization;

[ExcludeFromDescription]
public static class ConfigurationInitialization
{
    public static void BindConfigurationSections(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfigurationSection>(configuration.GetSection(JwtConfigurationSection.SectionName));
    }
}