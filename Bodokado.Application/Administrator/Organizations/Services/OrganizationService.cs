using Bodokado.Application.Administrator.Organizations.DTOs;
using Microsoft.EntityFrameworkCore;
using Bodokado.Application.Administrator.Organizations.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Constants;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace Bodokado.Application.Administrator.Organizations.Services;

public class OrganizationService : IOrganizationService
{
private readonly UserManager<User> _userManager;
private readonly RoleManager<IdentityRole<Guid>> _roleManager; // یا IdentityRole مطابق پروژه
private readonly IOrganizationRepository _repo;
private readonly IUnitOfWork _uow;

    public OrganizationService(
        IOrganizationRepository repo,
        IUnitOfWork uow,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _repo = repo;
        _uow = uow;
        _userManager = userManager;
        _roleManager = roleManager;
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

    public async Task<OrganizationAdminDto> CreateAdminAsync(
    Guid organizationId,
    CreateOrganizationAdminRequestDto request,
    CancellationToken ct = default)
{
    var org = await _repo.GetByIdAsync(organizationId, ct)
        ?? throw new NotFoundException(MessageKeys.NotFound, "organization_not_found");

    if (!org.IsActive)
        throw new BadRequestException(MessageKeys.BadRequest, "organization_inactive");

    var username = request.Username?.Trim();
    if (string.IsNullOrWhiteSpace(username))
        throw new BadRequestException(MessageKeys.BadRequest, "username_required");

    if (string.IsNullOrWhiteSpace(request.Password))
        throw new BadRequestException(MessageKeys.BadRequest, "password_required");

    var existing = await _userManager.FindByNameAsync(username);
    if (existing is not null)
        throw new BadRequestException(MessageKeys.UsernameAlreadyExists, "username_already_exists");

    // نقش
    if (!await _roleManager.RoleExistsAsync(RoleNames.AdminOrganization))
        await _roleManager.CreateAsync(new IdentityRole<Guid>(RoleNames.AdminOrganization));

    var user = new User
    {
        Id = Guid.NewGuid(),
        UserName = username,
        NormalizedUserName = _userManager.NormalizeName(username),
        Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
        PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
        FirstName = request.FirstName?.Trim(),
        LastName = request.LastName?.Trim(),
        OrganizationId = organizationId, // لینک فقط همین
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    if (!string.IsNullOrWhiteSpace(user.Email))
        user.NormalizedEmail = _userManager.NormalizeEmail(user.Email);

    var createResult = await _userManager.CreateAsync(user, request.Password);
    if (!createResult.Succeeded)
        throw new BadRequestException(
            string.Join(", ", createResult.Errors.Select(e => e.Description)),
            "create_org_admin_failed");

    var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.AdminOrganization);
    if (!roleResult.Succeeded)
        throw new BadRequestException(
            string.Join(", ", roleResult.Errors.Select(e => e.Description)),
            "assign_role_failed");

    return new OrganizationAdminDto
    {
        UserId = user.Id,
        Username = user.UserName!,
        PhoneNumber = user.PhoneNumber,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        OrganizationId = organizationId,
        OrganizationName = org.OrganizationName,
        IsActive = user.IsActive
    };
}

public async Task<List<OrganizationAdminDto>> GetAdminsAsync(Guid organizationId, CancellationToken ct = default)
{
    _ = await _repo.GetByIdAsync(organizationId, ct)
        ?? throw new NotFoundException(MessageKeys.NotFound, "organization_not_found");

    // کاربران این سازمان با نقش AdminOrganization
    var users = await _userManager.Users
        .Where(u => u.OrganizationId == organizationId && u.IsActive && !u.IsDeleted)
        .ToListAsync(ct);

    var result = new List<OrganizationAdminDto>();
    foreach (var u in users)
    {
        if (!await _userManager.IsInRoleAsync(u, RoleNames.AdminOrganization))
            continue;

        result.Add(new OrganizationAdminDto
        {
            UserId = u.Id,
            Username = u.UserName!,
            PhoneNumber = u.PhoneNumber,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            OrganizationId = organizationId,
            IsActive = u.IsActive
        });
    }

    return result;
}
}