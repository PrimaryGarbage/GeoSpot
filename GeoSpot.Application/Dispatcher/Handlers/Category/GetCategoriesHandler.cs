using GeoSpot.Application.Mappers;
using GeoSpot.Contracts.Category;
using GeoSpot.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Category;

public sealed class GetCategoriesRequest : IRequest<GetCategoriesResponseDto>;

internal class GetCategoriesHandler : IRequestHandler<GetCategoriesRequest, GetCategoriesResponseDto>
{
    private readonly GeoSpotDbContext _dbContext;

    public GetCategoriesHandler(GeoSpotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetCategoriesResponseDto> Handle(GetCategoriesRequest request, CancellationToken ct = default)
    {
        return new GetCategoriesResponseDto(await _dbContext.Categories.Select(x => x.MapToDto()).ToListAsync(ct));
    }
}