namespace Bodokado.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
