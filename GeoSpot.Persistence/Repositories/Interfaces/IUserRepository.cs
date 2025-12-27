using GeoSpot.Persistence.Repositories.Models.Category;
using GeoSpot.Persistence.Repositories.Models.User;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserModel> CreateUserAsync(CreateUserModel createModel, CancellationToken ct = default);
    
    Task<UserModel?> GetUserAsync(Guid userId, CancellationToken ct = default);
    
    Task<UserModel?> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);
    
    Task<UserModel?> UpdateUserAsync(UpdateUserModel updateModel, CancellationToken ct = default);
    
    Task<UserModel?> UpdateUserLocationAsync(double latitude, double longitude, CancellationToken ct = default);
}