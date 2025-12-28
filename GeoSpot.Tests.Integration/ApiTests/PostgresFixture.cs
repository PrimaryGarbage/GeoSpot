using Testcontainers.PostgreSql;

namespace GeoSpot.Tests.Integration.ApiTests;

public class PostgresFixture : IAsyncLifetime
{
    private const string DbName = "testdb";
    private const string DbUserName = "postgres";
    private const string DbUserPassword = "postgres";
    private const string PostGisImageName = "postgis/postgis:16-3.4";
    
    private PostgreSqlContainer Container { get; set; } = null!;
    public string ConnectionString => Container.GetConnectionString();
    
    public async Task InitializeAsync()
    {
        Container = new PostgreSqlBuilder()
            .WithDatabase(DbName)
            .WithUsername(DbUserName)
            .WithPassword(DbUserPassword)
            .WithImage(PostGisImageName)
            .Build();
        
        await Container.StartAsync();
    }

    public async Task DisposeAsync() => await Container.DisposeAsync();
}