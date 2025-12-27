using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Mappers;
using GeoSpot.Persistence.Repositories.Models.User;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Persistence.Repositories;

internal class UserRepository : BaseGeoSpotRepository, IUserRepository
{
    public UserRepository(GeoSpotDbContext dbContext) : base(dbContext)
    {}
    
    public Task<UserModel> CreateUserAsync(CreateUserModel createModel, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserModel?> GetUserAsync(Guid userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<UserModel?> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
    {
        UserEntity? entity = await DbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, ct);
        
        return entity.MapToModelOrNull();
    }

    public Task<UserModel?> UpdateUserAsync(UpdateUserModel updateModel, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserModel?> UpdateUserLocationAsync(double latitude, double longitude, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}