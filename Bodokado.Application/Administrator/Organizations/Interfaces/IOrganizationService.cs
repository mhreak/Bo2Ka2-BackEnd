using Bodokado.Application.Administrator.Organizations.DTOs;

namespace Bodokado.Application.Administrator.Organizations.Interfaces;

public interface IOrganizationService
{
    Task<OrganizationDto> CreateAsync(CreateOrganizationRequestDto request, CancellationToken ct = default);
    Task<OrganizationDto> UpdateAsync(Guid id, UpdateOrganizationRequestDto request, CancellationToken ct = default);
    Task<OrganizationDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<OrganizationDto>> GetAllAsync(bool? onlyActive = null, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}