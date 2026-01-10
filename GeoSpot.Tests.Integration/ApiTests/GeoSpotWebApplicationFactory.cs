using GeoSpot.Common.Constants;
using GeoSpot.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeoSpot.Tests.Integration.ApiTests;

[ExcludeFromCodeCoverage]
internal class GeoSpotWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    
    public GeoSpotWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable($"ConnectionStrings__{ConfigurationConstants.SqlDatabaseConnectionStringName}",
            _connectionString);
        
        builder.ConfigureServices(services =>
        {
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            GeoSpotDbContext dbContext = scope.ServiceProvider.GetRequiredService<GeoSpotDbContext>();
            dbContext.Database.Migrate();
        });
    }
}