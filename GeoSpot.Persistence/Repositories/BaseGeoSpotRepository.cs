namespace GeoSpot.Persistence.Repositories;

internal abstract class BaseGeoSpotRepository
{
    protected GeoSpotDbContext DbContext { get; init; }
    
    protected BaseGeoSpotRepository(GeoSpotDbContext dbContext)
    {
        DbContext = dbContext;
    }
}