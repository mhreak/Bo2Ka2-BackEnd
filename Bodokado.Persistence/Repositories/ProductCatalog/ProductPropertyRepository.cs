using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Domain.Entities.Products;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Products;

public class ProductPropertyRepository : BaseRepository<ProductAttribute>, IProductPropertyRepository
{
    public ProductPropertyRepository(AppDbContext context) : base(context) { }

    public async Task<ProductAttribute?> GetByIdWithCategoryAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.ProductProperties
            .Include(p => p.ProductCategory)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);
    }

    public async Task<List<ProductAttribute>> GetListAsync(Guid? productCategoryId, CancellationToken ct = default)
    {
        IQueryable<ProductAttribute> q = _context.ProductProperties
            .AsNoTracking()
            .Include(p => p.ProductCategory)
            .Where(p => !p.IsDeleted);

        if (productCategoryId.HasValue)
            q = q.Where(p => p.ProductCategoryId == productCategoryId);

        return await q.OrderBy(p => p.SortOrder).ThenBy(p => p.Name).ToListAsync(ct);
    }
}