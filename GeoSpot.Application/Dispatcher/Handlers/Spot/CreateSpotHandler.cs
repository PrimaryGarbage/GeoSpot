using GeoSpot.Application.Mappers;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record CreateSpotRequest(CreateSpotRequestDto Dto) : IRequest<SpotDto>;

internal class CreateSpotHandler : IRequestHandler<CreateSpotRequest, SpotDto>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public CreateSpotHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<SpotDto> Handle(CreateSpotRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        UserEntity user = await _dbContext.Users
                              .AsNoTracking()
                              .Include(x => x.BusinessProfiles)
                              .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
            ?? throw new NotFoundException(ErrorMessages.FailedToFindById<UserEntity>(userClaims.UserId));
        
        if (request.Dto.BusinessProfileId is not null)
        {
            if ((user.BusinessProfiles ?? []).All(x => x.BusinessProfileId != request.Dto.BusinessProfileId))
                throw new NotFoundException(ErrorMessages.FailedToFindById<BusinessProfileEntity>(request.Dto.BusinessProfileId));
        }
        
        SpotEntity spotEntity = _dbContext.Spots.Add(request.Dto.MapToEntity()).Entity;
        spotEntity.CreatorId = user.UserId;
        spotEntity.BusinessProfileId = request.Dto.BusinessProfileId;
        
        await _dbContext.SaveChangesAsync(ct);
        
        return spotEntity.MapToDto();
    }
}