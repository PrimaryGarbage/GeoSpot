namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface IUnitOfWork
{
    UnitOfWorkScope Start();
    
    bool IsInProcess();
    
    void Commit();
    
    Task CommitAsync(CancellationToken ct = default);
}