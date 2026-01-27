using GeoSpot.Application.Mappers;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record GetSpotByIdRequest(Guid SpotId) : IRequest<SpotDto>;

public class GetSpotByIdHandler : IRequestHandler<GetSpotByIdRequest, SpotDto>
{
    private readonly GeoSpotDbContext _dbContext;
    
    public GetSpotByIdHandler(GeoSpotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SpotDto> Handle(GetSpotByIdRequest request, CancellationToken ct = default)
    {
        SpotEntity entity = await _dbContext.Spots.AsNoTracking().FirstOrDefaultAsync(x => x.SpotId == request.SpotId, ct)
            ?? throw new NotFoundException($"Failed to find spot with the given ID. SpotId: {request.SpotId}");
        
        return entity.MapToDto();
    }
}