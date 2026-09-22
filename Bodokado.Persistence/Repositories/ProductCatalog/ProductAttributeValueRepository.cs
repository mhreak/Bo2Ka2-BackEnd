using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Domain.Entities.Products;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Products;

public class ProductAttributeValueRepository : BaseRepository<ProductAttributeValue>, IProductAttributeValueRepository
{
    public ProductAttributeValueRepository(AppDbContext context) : base(context) { }

    public async Task<ProductAttributeValue?> GetByIdWithAttributeAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.ProductAttributeValues
            .Include(v => v.ProductAttribute)
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, ct);
    }

    public async Task<List<ProductAttributeValue>> GetListAsync(Guid? productAttributeId, bool onlyActive, CancellationToken ct = default)
    {
        IQueryable<ProductAttributeValue> q = _context.ProductAttributeValues
            .AsNoTracking()
            .Include(v => v.ProductAttribute)
            .Where(v => !v.IsDeleted);

        if (productAttributeId.HasValue)
            q = q.Where(v => v.ProductAttributeId == productAttributeId);

        if (onlyActive)
            q = q.Where(v => v.IsActive);

        return await q.OrderBy(v => v.SortOrder).ThenBy(v => v.Title).ToListAsync(ct);
    }
}