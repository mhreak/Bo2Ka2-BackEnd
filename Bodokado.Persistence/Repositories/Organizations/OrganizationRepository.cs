using Bodokado.Application.Administrator.Organizations.Interfaces;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Bodokado.Persistence.Repositories.Organizations;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly AppDbContext _db;
    public OrganizationRepository(AppDbContext db) => _db = db;

    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Set<Organization>()
            .Include(o => o.LogoFile)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted, ct);

    public Task<List<Organization>> GetAllAsync(bool? onlyActive = null, CancellationToken ct = default)
    {
        var q = _db.Set<Organization>()
            .Include(o => o.LogoFile)
            .Where(o => !o.IsDeleted);

        if (onlyActive == true)
            q = q.Where(o => o.IsActive);

        return q.OrderBy(o => o.OrganizationName).ToListAsync(ct);
    }

    public Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        var q = _db.Set<Organization>()
            .Where(o => !o.IsDeleted && o.OrganizationName == name);

        if (excludeId.HasValue)
            q = q.Where(o => o.Id != excludeId.Value);

        return q.AnyAsync(ct);
    }

    public async Task AddAsync(Organization entity, CancellationToken ct = default)
        => await _db.Set<Organization>().AddAsync(entity, ct);

    public void Update(Organization entity)
        => _db.Set<Organization>().Update(entity);
}