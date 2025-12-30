namespace GeoSpot.Tests.Integration.ApiTests;

public interface IGeoSpotWebApplicationFactory
{
    static abstract IGeoSpotWebApplicationFactory Create(string connectionString);
    
    HttpClient CreateClient();
}