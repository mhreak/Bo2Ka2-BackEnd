using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;

public interface IProductAttributeRepository : IGenericRepository<ProductAttribute>
{
    Task<ProductAttribute?> GetByIdWithCategoryAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductAttribute>> GetListAsync(Guid? productCategoryId, CancellationToken ct = default);
}