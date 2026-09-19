using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Domain.Entities.Products;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Products;

public class ProductCategoryRepository : BaseRepository<ProductCategory>, IProductCategoryRepository
{
    public ProductCategoryRepository(AppDbContext context) : base(context) { }

    public async Task<ProductCategory?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.ProductCategories
            .Include(c => c.ParentCategory)
            .Include(c => c.Image)
            .Include(c => c.Children.Where(ch => !ch.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct);
    }

    public async Task<List<ProductCategory>> GetAllWithDetailsAsync(bool onlyActive, CancellationToken ct = default)
    {
        IQueryable<ProductCategory> q = _context.ProductCategories
            .AsNoTracking()
            .Include(c => c.Image)
            .Include(c => c.ParentCategory)
            .Where(c => !c.IsDeleted);

        if (onlyActive)
            q = q.Where(c => c.IsActive);

        return await q.OrderBy(c => c.Name).ToListAsync(ct);
    }

    public Task<bool> HasChildrenAsync(Guid id, CancellationToken ct = default)
        => _context.ProductCategories.AnyAsync(c => c.ParentCategoryId == id && !c.IsDeleted, ct);

    public async Task<bool> IsDescendantOfAsync(Guid categoryId, Guid potentialAncestorId, CancellationToken ct = default)
    {
        var currentId = categoryId;
        var guard = 0;
        while (guard++ < 50)
        {
            var parentId = await _context.ProductCategories
                .Where(c => c.Id == currentId && !c.IsDeleted)
                .Select(c => c.ParentCategoryId)
                .FirstOrDefaultAsync(ct);

            if (parentId is null) return false;
            if (parentId == potentialAncestorId) return true;
            currentId = parentId.Value;
        }
        return false;
    }
}