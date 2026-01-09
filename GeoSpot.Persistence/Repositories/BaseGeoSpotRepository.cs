using GeoSpot.Persistence.Repositories.Interfaces;

namespace GeoSpot.Persistence.Repositories;

internal abstract class BaseGeoSpotRepository
{
    protected GeoSpotDbContext DbContext { get; init; }
    private readonly IUnitOfWork _unitOfWork;
    
    protected BaseGeoSpotRepository(GeoSpotDbContext dbContext, IUnitOfWork unitOfWork)
    {
        DbContext = dbContext;
        _unitOfWork = unitOfWork;
    }
    
    protected void SaveChanges()
    {
        if (!_unitOfWork.IsInProcess())
            DbContext.SaveChanges();
    }
    
    protected async Task SaveChangesAsync(CancellationToken ct = default)
    {
        if (!_unitOfWork.IsInProcess())
            await DbContext.SaveChangesAsync(ct);
    }
}