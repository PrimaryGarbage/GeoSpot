namespace GeoSpot.Persistence.Repositories;

internal class BaseGeoSpotRepository
{
    protected GeoSpotDbContext DbContext { get; init; }
    
    public BaseGeoSpotRepository(GeoSpotDbContext dbContext)
    {
        DbContext = dbContext;
    }
}