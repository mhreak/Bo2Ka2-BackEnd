using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Domain.Entities.Products;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Products;

public class ProductProductAttributeRepository : BaseRepository<ProductProductAttribute>, IProductProductAttributeRepository
{
    public ProductProductAttributeRepository(AppDbContext context) : base(context) { }

    public async Task<ProductProductAttribute?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Product_ProductAttributes
            .Include(x => x.Product)
            .Include(x => x.ProductAttribute)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
    }

    public async Task<List<ProductProductAttribute>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.Product_ProductAttributes
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.ProductAttribute)
            .Where(x => x.ProductId == productId && !x.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task SoftDeleteByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        var items = await _context.Product_ProductAttributes
            .Where(x => x.ProductId == productId && !x.IsDeleted)
            .ToListAsync(ct);

        foreach (var item in items)
        {
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
        }
    }
}