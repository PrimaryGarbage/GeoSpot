using GeoSpot.Application.Mappers;
using GeoSpot.Contracts.ReactionType;
using GeoSpot.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.ReactionType;

public record GetReactionTypesRequest() : IRequest<GetReactionTypesResponseDto>;

internal class GetReactionTypesHandler : IRequestHandler<GetReactionTypesRequest, GetReactionTypesResponseDto>
{
    private readonly GeoSpotDbContext _dbContext;

    public GetReactionTypesHandler(GeoSpotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetReactionTypesResponseDto> Handle(GetReactionTypesRequest request, CancellationToken ct = default)
    {
        return new GetReactionTypesResponseDto
        {
            ReactionTypes = await _dbContext.ReactionTypes.Select(x => x.MapToDto()).ToListAsync(ct)
        };
    }
}