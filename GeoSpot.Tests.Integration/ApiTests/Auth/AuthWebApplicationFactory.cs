using System.Diagnostics.CodeAnalysis;
using GeoSpot.Common.Constants;
using GeoSpot.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeoSpot.Tests.Integration.ApiTests.Auth;

[ExcludeFromCodeCoverage]
public class AuthWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public AuthWebApplicationFactory(string connectionString)
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
            
            // Seed test data using dbContext here
        });
    }
}