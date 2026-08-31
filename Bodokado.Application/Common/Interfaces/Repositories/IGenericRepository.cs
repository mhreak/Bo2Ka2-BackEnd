namespace Bodokado.Application.Common.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<Bodokado.Application.Common.Pagination.PagedResult<T>> GetPagedAsync(Bodokado.Application.Common.Pagination.PaginationQuery query, System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null);
    Task<int> CountAsync();
    Task<T?> GetByIdAsync(object id);
    Task<List<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync();
}
