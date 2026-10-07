using Bodokado.Application.Administrator.Organizations.DTOs;
using Bodokado.Application.Administrator.Organizations.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Organizations;

namespace Bodokado.Application.Administrator.Organizations.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _repo;
    private readonly IUnitOfWork _uow;

    public OrganizationService(IOrganizationRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<OrganizationDto> CreateAsync(CreateOrganizationRequestDto request, CancellationToken ct = default)
    {
        var name = request.OrganizationName?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new BadRequestException(MessageKeys.BadRequest, "organization_name_required");

        if (await _repo.NameExistsAsync(name, null, ct))
            throw new BadRequestException(MessageKeys.BadRequest, "organization_name_exists");

        var entity = new Organization
        {
            Id = Guid.NewGuid(),
            OrganizationName = name,
            IsActive = request.IsActive,
            LogoFileId = request.LogoFileId,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return await GetByIdAsync(entity.Id, ct);
    }

    public async Task<OrganizationDto> UpdateAsync(Guid id, UpdateOrganizationRequestDto request, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.NotFound, "organization_not_found");

        var name = request.OrganizationName?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new BadRequestException(MessageKeys.BadRequest, "organization_name_required");

        if (await _repo.NameExistsAsync(name, id, ct))
            throw new BadRequestException(MessageKeys.BadRequest, "organization_name_exists");

        entity.OrganizationName = name;
        entity.IsActive = request.IsActive;
        entity.LogoFileId = request.LogoFileId;
        entity.UpdatedAt = DateTime.UtcNow;

        _repo.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<OrganizationDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.NotFound, "organization_not_found");
        return Map(entity);
    }

    public async Task<List<OrganizationDto>> GetAllAsync(bool? onlyActive = null, CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(onlyActive, ct);
        return list.Select(Map).ToList();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.NotFound, "organization_not_found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _repo.Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    private static OrganizationDto Map(Organization o) => new()
    {
        Id = o.Id,
        OrganizationName = o.OrganizationName,
        IsActive = o.IsActive,
        LogoFileId = o.LogoFileId,
        LogoPath = o.LogoFile?.Path,
        CreatedAt = o.CreatedAt,
        UpdatedAt = o.UpdatedAt
    };
}