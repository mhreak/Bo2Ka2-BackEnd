using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Products;

public interface IProductPropertyValueRepository : IGenericRepository<ProductPropertyValue>
{
    Task<ProductPropertyValue?> GetByIdWithPropertyAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductPropertyValue>> GetListAsync(Guid? productPropertyId, bool onlyActive, CancellationToken ct = default);
}