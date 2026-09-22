using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;

public interface IProductProductAttributeRepository : IGenericRepository<ProductProductAttribute>
{
    Task<ProductProductAttribute?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductProductAttribute>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task SoftDeleteByProductIdAsync(Guid productId, CancellationToken ct = default);
}