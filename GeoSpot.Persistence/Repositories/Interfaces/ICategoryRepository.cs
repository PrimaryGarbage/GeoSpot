using GeoSpot.Persistence.Repositories.Models.Category;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync(CancellationToken ct = default);
    
    Task<IEnumerable<CategoryModel>> GetUserCategoriesAsync(Guid userId, CancellationToken ct = default);
    
    Task UpdateUserCategoriesAsync(Guid userId, IEnumerable<Guid> categoryIds, CancellationToken ct = default);
}