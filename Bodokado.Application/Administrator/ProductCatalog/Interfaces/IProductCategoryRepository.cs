using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;

public interface IProductCategoryRepository : IGenericRepository<ProductCategory>
{
    Task<ProductCategory?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductCategory>> GetAllWithDetailsAsync(bool onlyActive, CancellationToken ct = default);
    Task<bool> HasChildrenAsync(Guid id, CancellationToken ct = default);
    Task<bool> IsDescendantOfAsync(Guid categoryId, Guid potentialAncestorId, CancellationToken ct = default);
}