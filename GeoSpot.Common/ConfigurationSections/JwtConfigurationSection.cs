namespace GeoSpot.Common.ConfigurationSections;

public class JwtConfigurationSection : IConfigurationSection
{
    public static string SectionName => "Jwt";
    
    public required string Issuer { get; init; }
    
    public required string Audience { get; init; }
    
    public required string Key { get; init; }
    
    public required int AccessTokenLifespanMinutes { get; init; }
    
    public required int RefreshTokenLifespanMinutes { get; init; }
}