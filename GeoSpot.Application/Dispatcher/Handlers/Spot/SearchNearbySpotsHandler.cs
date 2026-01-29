using GeoSpot.Application.Mappers;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities.Factories;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record SearchNearbySpotsRequest(SearchNearbySpotsRequestDto Dto) : IRequest<SearchNearbySpotsResponseDto>;

internal class SearchNearbySpotsHandler : IRequestHandler<SearchNearbySpotsRequest, SearchNearbySpotsResponseDto>
{
    private readonly GeoSpotDbContext _dbContext;

    public SearchNearbySpotsHandler(GeoSpotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SearchNearbySpotsResponseDto> Handle(SearchNearbySpotsRequest request, CancellationToken ct = default)
    {
        Point userPosition = GeographyFactory.CreatePoint(request.Dto.Latitude, request.Dto.Longitude);
        var spotsWithViewCounts = await _dbContext.Spots
            .AsNoTracking()
            .Where(x => x.Position.IsWithinDistance(userPosition, request.Dto.Radius))
            .Select(x => new { Spot = x, ViewsCount = x.UserViews!.Count() })
            .ToListAsync(ct);
        
        return new SearchNearbySpotsResponseDto { Spots = spotsWithViewCounts.Select(x => x.Spot.MapToDto(x.ViewsCount)) };
    }
}