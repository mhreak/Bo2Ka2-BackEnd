using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Products;

public interface IProductProductPropertyRepository : IGenericRepository<ProductProductProperty>
{
    Task<ProductProductProperty?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductProductProperty>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task SoftDeleteByProductIdAsync(Guid productId, CancellationToken ct = default);
}