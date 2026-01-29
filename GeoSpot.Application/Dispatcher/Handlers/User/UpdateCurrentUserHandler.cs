using GeoSpot.Application.Mappers;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;

namespace GeoSpot.Application.Dispatcher.Handlers.User;


public record UpdateCurrentUserRequest(UpdateCurrentUserRequestDto Dto) : IRequest<Empty>;

internal class UpdateCurrentUserHandler : IRequestHandler<UpdateCurrentUserRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public UpdateCurrentUserHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(UpdateCurrentUserRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        UserEntity userEntity = await _dbContext.Users.FindAsync([userClaims.UserId], ct)
            ?? throw new NotFoundException(ErrorMessages.FailedToFindById<UserEntity>(userClaims.UserId));
        
        request.Dto.MapOntoEntity(userEntity);
        
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}