// Persistence/Repositories/Settings/SettingRepository.cs
using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.AdminModule.Settings;
using Bodokado.Domain.Entities.Settings;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;
using Bodokado.Application.Administrator.Settings.Interfaces;

namespace Bodokado.Persistence.Repositories.Settings;

public class SettingRepository : BaseRepository<Setting>, ISettingRepository
{
    public SettingRepository(AppDbContext context) : base(context) { }

    public Task<Setting?> GetByKeyAsync(string key, CancellationToken ct = default)
        => _context.Settings.FirstOrDefaultAsync(x => x.Key == key && !x.IsDeleted, ct);
}