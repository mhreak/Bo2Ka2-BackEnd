using Bodokado.Domain.Entities;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.File.Interfaces;

public interface IFileAssetRepository : Bodokado.Application.Common.Interfaces.Repositories.IGenericRepository<FileAsset>
{
    Task<FileAsset?> GetByIdAsync(Guid id);
    Task<List<FileAsset>> GetByUploaderIdAsync(Guid uploaderId);
    Task<List<FileAsset>> GetByUploaderIdAndTypeAsync(Guid uploaderId, UploadFileType fileType);
}
