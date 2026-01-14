namespace GeoSpot.Common.ConfigurationSections;

public class GeolocationConfigurationSection : IConfigurationSection
{
    public static string SectionName { get; } = "Geolocation";
    
    public int MaxSearchDistance { get; init; }
    
    public int MaxSpotRadius { get; init; }
}