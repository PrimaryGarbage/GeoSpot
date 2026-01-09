using GeoSpot.Persistence.Repositories.Interfaces;

namespace GeoSpot.Persistence.Repositories;

[ExcludeFromCodeCoverage]
public sealed class UnitOfWorkScope : IAsyncDisposable
{
    private readonly IUnitOfWork _parent;

    public UnitOfWorkScope(IUnitOfWork parent)
    {
        _parent = parent;
    }

    public async ValueTask DisposeAsync()
    {
        await _parent.CommitAsync();
    }
}