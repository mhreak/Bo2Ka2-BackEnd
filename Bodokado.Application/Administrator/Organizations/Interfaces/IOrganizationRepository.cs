using Bodokado.Domain.Entities.Organizations;

namespace Bodokado.Application.Administrator.Organizations.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Organization>> GetAllAsync(bool? onlyActive = null, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
    Task AddAsync(Organization entity, CancellationToken ct = default);
    void Update(Organization entity);
}