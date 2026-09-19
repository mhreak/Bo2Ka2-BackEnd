using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;

public interface IProductPropertyRepository : IGenericRepository<ProductProperty>
{
    Task<ProductProperty?> GetByIdWithCategoryAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductProperty>> GetListAsync(Guid? productCategoryId, CancellationToken ct = default);
}