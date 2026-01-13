using GeoSpot.Application.Mappers;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.User;

public class GetCurrentUserCategoriesRequest : IRequest<GetCurrentUserCategoriesResponseDto>;

public class GetCurrentUserCategoriesHandler : IRequestHandler<GetCurrentUserCategoriesRequest, GetCurrentUserCategoriesResponseDto>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public GetCurrentUserCategoriesHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<GetCurrentUserCategoriesResponseDto> Handle(GetCurrentUserCategoriesRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        UserEntity user = await _dbContext.Users
            .AsNoTracking()
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
            ?? throw new NotFoundException($"Failed to find user with the given Id. UserId: {userClaims.UserId}");
        
        return new GetCurrentUserCategoriesResponseDto { Categories = user.Categories?.Select(x => x.MapToDto()) ?? [] };
    }
}