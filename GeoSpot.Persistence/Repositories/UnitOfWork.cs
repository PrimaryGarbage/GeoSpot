using GeoSpot.Persistence.Repositories.Interfaces;

namespace GeoSpot.Persistence.Repositories;

[ExcludeFromCodeCoverage]
internal class UnitOfWork : IUnitOfWork
{
    private readonly GeoSpotDbContext _dbContext;
    private bool _inProcess;

    public UnitOfWork(GeoSpotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public UnitOfWorkScope Start()
    {
        _inProcess = true;
        return new UnitOfWorkScope(this);
    }

    public bool IsInProcess() => _inProcess;

    public void Commit()
    {
        _dbContext.SaveChanges();
        _inProcess = false;
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
        _inProcess = false;
    }
}