using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Products;

public interface IProductAttributeValueRepository : IGenericRepository<ProductAttributeValue>
{
    Task<ProductAttributeValue?> GetByIdWithAttributeAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductAttributeValue>> GetListAsync(Guid? productPropertyId, bool onlyActive, CancellationToken ct = default);
}