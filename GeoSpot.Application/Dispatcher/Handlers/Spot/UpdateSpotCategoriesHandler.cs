using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public sealed record UpdateSpotCategoriesRequest(Guid SpodId, UpdateSpotCategoriesRequestDto Dto) : IRequest<Empty>;

internal sealed class UpdateSpotCategoriesHandler : IRequestHandler<UpdateSpotCategoriesRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public UpdateSpotCategoriesHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(UpdateSpotCategoriesRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        
        SpotEntity spot = await _dbContext.Spots
                            .Include(x => x.Categories)
                            .FirstOrDefaultAsync(x => x.SpotId == request.SpodId && x.CreatorId == userClaims.UserId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<SpotEntity>(request.SpodId));
        
        if (!request.Dto.CategoryIds.Any())
        {
            spot.Categories!.Clear();
        }
        else
        {
            List<CategoryEntity> requestedCategories = await _dbContext.Categories.Where(x => request.Dto.CategoryIds.Contains(x.CategoryId)).ToListAsync(ct);
            if (requestedCategories.Count != request.Dto.CategoryIds.Count)
            {
                List<Guid> validIds = requestedCategories.Select(x => x.CategoryId).ToList();
                List<Guid> invalidIds = request.Dto.CategoryIds.Where(x => !validIds.Contains(x)).ToList();
                throw new NotFoundException($"Failed to find categories with the provided IDs. Invalid IDs: {string.Join(", ", invalidIds)}");
            }
            
            spot.Categories = requestedCategories;
        }
        
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}