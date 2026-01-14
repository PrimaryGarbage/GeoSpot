using GeoSpot.Application.Mappers;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Entities.Factories;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record SearchNearbySpotsRequest(SearchNearbySpotsRequestDto Dto) : IRequest<SearchNearbySpotsResponseDto>;

public class SearchNearbySpotsHandler : IRequestHandler<SearchNearbySpotsRequest, SearchNearbySpotsResponseDto>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public SearchNearbySpotsHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<SearchNearbySpotsResponseDto> Handle(SearchNearbySpotsRequest request, CancellationToken ct = default)
    {
        Point userPosition = GeographyFactory.CreatePoint(request.Dto.Latitude, request.Dto.Longitude);
        List<SpotEntity> spots = await _dbContext.Spots.Where(x => x.Position.IsWithinDistance(userPosition, request.Dto.Radius))
            .ToListAsync(ct);
        
        return new SearchNearbySpotsResponseDto { Spots = spots.Select(x => x.MapToDto() ) };
    }
}