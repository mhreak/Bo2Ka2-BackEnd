using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Domain.Entities.Products;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Products;

public class ProductPropertyValueRepository : BaseRepository<ProductPropertyValue>, IProductPropertyValueRepository
{
    public ProductPropertyValueRepository(AppDbContext context) : base(context) { }

    public async Task<ProductPropertyValue?> GetByIdWithPropertyAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.ProductPropertyValues
            .Include(v => v.ProductProperty)
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, ct);
    }

    public async Task<List<ProductPropertyValue>> GetListAsync(Guid? productPropertyId, bool onlyActive, CancellationToken ct = default)
    {
        IQueryable<ProductPropertyValue> q = _context.ProductPropertyValues
            .AsNoTracking()
            .Include(v => v.ProductProperty)
            .Where(v => !v.IsDeleted);

        if (productPropertyId.HasValue)
            q = q.Where(v => v.ProductPropertyId == productPropertyId);

        if (onlyActive)
            q = q.Where(v => v.IsActive);

        return await q.OrderBy(v => v.SortOrder).ThenBy(v => v.Title).ToListAsync(ct);
    }
}