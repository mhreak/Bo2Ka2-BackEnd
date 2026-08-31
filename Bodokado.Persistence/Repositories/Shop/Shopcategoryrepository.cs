using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.ShopModule.Registration.Interfaces;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Repositories.Shops;

public class ShopCategoryRepository : BaseRepository<ShopCategory>, IShopCategoryRepository
{
    public ShopCategoryRepository(AppDbContext context) : base(context) { }

    public async Task<List<ShopCategory>> SearchAsync(string? search, CancellationToken ct = default)
    {
        var query = _context.ShopCategories.Where(c => c.IsActive && !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => c.Name.Contains(term));
        }

        return await query
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);
    }
}