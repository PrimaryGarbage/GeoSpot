using GeoSpot.Persistence.Repositories.Models.Category;
using GeoSpot.Persistence.Repositories.Models.User;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserModel> CreateUserAsync(CreateUserModel createModel, CancellationToken ct);
    
    Task<UserModel> GetUserAsync(Guid userId, CancellationToken ct);
    
    Task<UserModel> UpdateUserAsync(UpdateUserModel updateModel, CancellationToken ct);
    
    Task UpdateUserLocationAsync(double latitude, double longitude, CancellationToken ct);
}