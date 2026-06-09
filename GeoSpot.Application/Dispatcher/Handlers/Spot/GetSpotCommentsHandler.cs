using GeoSpot.Application.Mappers;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record GetSpotCommentsRequest(Guid SpotId) : IRequest<GetSpotCommentsResponseDto>;

internal class GetSpotCommentsHandler : IRequestHandler<GetSpotCommentsRequest, GetSpotCommentsResponseDto>
{
    private readonly GeoSpotDbContext _dbContext;

    public GetSpotCommentsHandler(GeoSpotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetSpotCommentsResponseDto> Handle(GetSpotCommentsRequest request, CancellationToken ct = default)
    {
        SpotEntity spot = await _dbContext.Spots
                              .AsNoTracking()
                              .Include(x => x.Comments)
                              .FirstOrDefaultAsync(x => x.SpotId == request.SpotId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<SpotEntity>(request.SpotId));
        
        return new(spot.Comments!.Select(x => x.MapToDto()).ToList());
    }
}