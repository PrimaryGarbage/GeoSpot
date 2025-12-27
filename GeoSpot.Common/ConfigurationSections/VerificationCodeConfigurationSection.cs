namespace GeoSpot.Common.ConfigurationSections;

public class VerificationCodeConfigurationSection : IConfigurationSection
{
    public static string SectionName => "VerificationCode";

    public int LifespanSeconds { get; init; }
    
    public int NumberOfDigits { get; init; }
}