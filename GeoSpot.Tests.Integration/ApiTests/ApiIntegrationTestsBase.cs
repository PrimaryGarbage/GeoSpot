using GeoSpot.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests;

[ExcludeFromCodeCoverage]
public abstract class ApiIntegrationTestsBase : IAsyncLifetime
{
    protected HttpClient Client { get; init; }
    
    private protected GeoSpotDbContext DbContext { get; init; }
    
    private protected ApiIntegrationTestsBase(HttpClient httpClient, GeoSpotDbContext dbContext)
    {
        Client = httpClient;
        DbContext = dbContext;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return ResetDatabaseAsync();
    }

    private async Task ResetDatabaseAsync()
    {
        List<string> tables = DbContext.Model.GetEntityTypes()
            .Where(t => t.GetTableName() is not null)
            .Select(t =>
            {
                string schema = t.GetSchema() ?? "public";
                string? name = t.GetTableName();
                return $"\"{schema}\".\"{name}\"";
            })
            .Distinct()
            .ToList();

        if (tables.Count == 0) return;

        string sql = $"TRUNCATE TABLE {string.Join(", ", tables)} RESTART IDENTITY CASCADE;";
        await DbContext.Database.ExecuteSqlRawAsync(sql);
    }
}