using Microsoft.EntityFrameworkCore;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Domain.Entities;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.File;

public class FileAssetRepository : BaseRepository<FileAsset>, IFileAssetRepository
{
    public FileAssetRepository(AppDbContext context) : base(context) { }

    public async Task<FileAsset?> GetByIdAsync(Guid id)
    {
        var results = await FindAsync(file => file.Id == id && !file.IsDeleted);
        return results.FirstOrDefault();
    }

    public async Task<List<FileAsset>> GetByUploaderIdAsync(Guid uploaderId)
    {
        return await FindAsync(file => file.UploaderId == uploaderId && !file.IsDeleted);
    }

    public async Task<List<FileAsset>> GetByUploaderIdAndTypeAsync(Guid uploaderId, UploadFileType fileType)
    {
        return await _context.Set<FileAsset>().Where(f => f.UploaderId == uploaderId && f.FileType == fileType && !f.IsDeleted).ToListAsync();
    }
}
