using GeoSpot.Persistence;

namespace GeoSpot.Tests.Integration.ApiTests;

[ExcludeFromCodeCoverage]
public abstract class ApiIntegrationTestsBase
{
    protected HttpClient Client { get; init; }
    
    private protected GeoSpotDbContext DbContext { get; init; }
    
    private readonly List<Action<GeoSpotDbContext>> _cleanupActions = [];

    private protected ApiIntegrationTestsBase(HttpClient httpClient, GeoSpotDbContext dbContext)
    {
        Client = httpClient;
        DbContext = dbContext;
    }

    private protected void AddToCleanup(Action<GeoSpotDbContext> action)
    {
        _cleanupActions.Add(action);
    }
    
    private protected async Task Cleanup()
    {
        _cleanupActions.ForEach(x => x.Invoke(DbContext));
        _cleanupActions.Clear();
        await DbContext.SaveChangesAsync();
    }
}