using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;

public interface IProductAttributeValueRepository : IGenericRepository<ProductAttributeValue>
{
    Task<ProductAttributeValue?> GetByIdWithAttributeAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductAttributeValue>> GetListAsync(Guid? productAttributeId, bool onlyActive, CancellationToken ct = default);
}