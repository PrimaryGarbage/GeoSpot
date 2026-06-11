using GeoSpot.Application.Mappers;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public sealed record GetSpotCategoriesRequest(Guid SpotId) : IRequest<GetSpotCategoriesResponseDto>;

internal sealed class GetSpotCategoriesHandler : IRequestHandler<GetSpotCategoriesRequest, GetSpotCategoriesResponseDto>
{
    private readonly GeoSpotDbContext _dbContext;

    public GetSpotCategoriesHandler(GeoSpotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetSpotCategoriesResponseDto> Handle(GetSpotCategoriesRequest request, CancellationToken ct = default)
    {
        SpotEntity spot = await _dbContext.Spots
                            .AsNoTracking()
                            .Include(x => x.Categories)
                            .FirstOrDefaultAsync(x => x.SpotId == request.SpotId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<SpotEntity>(request.SpotId));
        
        return new GetSpotCategoriesResponseDto(spot.Categories!.Select(x => x.MapToDto()));
    }
}