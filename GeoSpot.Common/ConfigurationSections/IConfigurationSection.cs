namespace GeoSpot.Common.ConfigurationSections;

public interface IConfigurationSection
{
    static abstract string SectionName { get; }
}