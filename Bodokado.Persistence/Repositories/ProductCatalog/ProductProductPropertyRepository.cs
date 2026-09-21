using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Domain.Entities.Products;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Products;

public class ProductProductPropertyRepository : BaseRepository<ProductAttributeValue>, IProductProductPropertyRepository
{
    public ProductProductPropertyRepository(AppDbContext context) : base(context) { }

    public async Task<ProductAttributeValue?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Product_ProductProperties
            .Include(x => x.Product)
            .Include(x => x.ProductProperty)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
    }

    public async Task<List<ProductAttributeValue>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.Product_ProductProperties
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.ProductProperty)
            .Where(x => x.ProductId == productId && !x.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task SoftDeleteByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        var items = await _context.Product_ProductProperties
            .Where(x => x.ProductId == productId && !x.IsDeleted)
            .ToListAsync(ct);

        foreach (var item in items)
        {
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
        }
    }
}