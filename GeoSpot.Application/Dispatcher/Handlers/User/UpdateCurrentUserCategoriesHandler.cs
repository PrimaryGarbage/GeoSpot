using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.User;

public record UpdateCurrentUserCategoriesRequest(UpdateCurrentUserCategoriesRequestDto Dto) : IRequest<Empty>;

public class UpdateCurrentUserCategoriesHandler : IRequestHandler<UpdateCurrentUserCategoriesRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public UpdateCurrentUserCategoriesHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(UpdateCurrentUserCategoriesRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        UserEntity user = await _dbContext.Users
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
            ?? throw new NotFoundException($"Failed to find user with the given Id. UserId: {userClaims.UserId}");
        
        List<Guid> categoryIdsToAdd = request.Dto.Categories.Select(x => x.CategoryId).ToList();
        List<CategoryEntity> categoriesToAdd = await _dbContext.Categories
            .Where(x => categoryIdsToAdd.Contains(x.CategoryId))
            .ToListAsync(ct);
        
        if (categoriesToAdd.Count < categoryIdsToAdd.Count)
        {
            IEnumerable<Guid> invalidCategoryIds = categoryIdsToAdd.Where(c => !categoriesToAdd.Select(ec => ec.CategoryId).Contains(c));
            throw new NotFoundException($"Failed to find some of the given categories. Invalid category IDs: {string.Join(", ", invalidCategoryIds)}");
        }
        
        user.Categories = categoriesToAdd;
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}