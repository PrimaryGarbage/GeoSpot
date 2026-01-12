using GeoSpot.Common.Enums;
using GeoSpot.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace GeoSpot.Tests.Integration.ApiTests;


public class ApiIntegrationFixture : IAsyncLifetime
{
    private const string DbName = "testdb";
    private const string DbUserName = "postgres";
    private const string DbUserPassword = "postgres";
    private const string PostGisImageName = "postgis/postgis:16-3.4";
    
    private PostgreSqlContainer Container { get; set; } = null!;
    
    private GeoSpotWebApplicationFactory _factory = null!;
    
    public HttpClient AuthorizedHttpClient { get; private set; } = null!;
    
    internal GeoSpotDbContext DbContext { get; private set; } = null!;
    
    public IServiceProvider Services { get; private set; } = null!;
    
    public HttpClient CreateHttpClient() => _factory.CreateClient();
    
    public async Task InitializeAsync()
    {
        Container = new PostgreSqlBuilder()
            .WithDatabase(DbName)
            .WithUsername(DbUserName)
            .WithPassword(DbUserPassword)
            .WithImage(PostGisImageName)
            .Build();
        
        await Container.StartAsync();
        
        _factory = new GeoSpotWebApplicationFactory(Container.GetConnectionString());
        Services = _factory.Services;
        
        var dbOptions = new DbContextOptionsBuilder<GeoSpotDbContext>()
            .UseNpgsql(Container.GetConnectionString(), o =>
            {
                o.UseNetTopologySuite(geographyAsDefault: true);
                o.MigrationsHistoryTable("__EFMigrationHistory", GeoSpotDbContext.DefaultSchema);
                o.MapEnum<AccountType>(schemaName: GeoSpotDbContext.DefaultSchema);
                o.MapEnum<Gender>(schemaName: GeoSpotDbContext.DefaultSchema);
                o.MapEnum<SpotType>(schemaName: GeoSpotDbContext.DefaultSchema);
                o.MapEnum<Platform>(schemaName: GeoSpotDbContext.DefaultSchema);
            })
            .UseSnakeCaseNamingConvention()
            .Options;
        
        DbContext = new GeoSpotDbContext(dbOptions);
    }

    public async Task DisposeAsync() => await Container.DisposeAsync();   
}