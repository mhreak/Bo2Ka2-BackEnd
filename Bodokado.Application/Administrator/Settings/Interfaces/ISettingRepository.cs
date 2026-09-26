using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Settings;

namespace Bodokado.Application.Administrator.Settings.Interfaces;

public interface ISettingRepository : IGenericRepository<Setting>
{
    Task<Setting?> GetByKeyAsync(string key, CancellationToken ct = default);
}