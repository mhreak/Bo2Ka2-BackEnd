using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Products;

public interface IProductProductAttributeRepository : IGenericRepository<ProductAttributeValue>
{
    Task<ProductAttributeValue?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductAttributeValue>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task SoftDeleteByProductIdAsync(Guid productId, CancellationToken ct = default);
}