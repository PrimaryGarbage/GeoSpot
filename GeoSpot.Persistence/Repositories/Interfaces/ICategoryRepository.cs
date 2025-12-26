using GeoSpot.Persistence.Repositories.Models.Category;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync(CancellationToken ct);
    
    Task<IEnumerable<CategoryModel>> GetUserCategoriesAsync(Guid userId, CancellationToken ct);
    
    Task UpdateUserCategoriesAsync(Guid userId, IEnumerable<Guid> categoryIds, CancellationToken ct);
}