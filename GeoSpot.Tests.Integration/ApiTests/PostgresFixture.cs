using Testcontainers.PostgreSql;

namespace GeoSpot.Tests.Integration.ApiTests;

[ExcludeFromCodeCoverage]
[CollectionDefinition("AuthTest")]
public class PostgresFixture<TApplicationFactory> : IAsyncLifetime where TApplicationFactory : class, IGeoSpotWebApplicationFactory
{
    private const string DbName = "testdb";
    private const string DbUserName = "postgres";
    private const string DbUserPassword = "postgres";
    private const string PostGisImageName = "postgis/postgis:16-3.4";
    
    private PostgreSqlContainer Container { get; set; } = null!;
    
    private IGeoSpotWebApplicationFactory _factory = null!;
    
    public HttpClient HttpClient { get; private set; } = null!;
    
    public async Task InitializeAsync()
    {
        Container = new PostgreSqlBuilder()
            .WithDatabase(DbName)
            .WithUsername(DbUserName)
            .WithPassword(DbUserPassword)
            .WithImage(PostGisImageName)
            .Build();
        
        await Container.StartAsync();
        
        _factory = TApplicationFactory.Create(Container.GetConnectionString());
        HttpClient = _factory.CreateClient();
    }

    public async Task DisposeAsync() => await Container.DisposeAsync();   
}